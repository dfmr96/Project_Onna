using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Melee
{
    public class MeleeController : MonoBehaviour
    {
        public event Action<int> OnComboStepChanged;
        public event Action OnComboReset;

        [Header("Data")]
        [SerializeField] private MeleeData data;
        [SerializeField] private Transform attackPoint;
        private MeleeModel Model { get; set; }

        public event Action OnCooldownComplete;
        public bool CanAttack => !_onCoolDown && ComboStep < 2;
        public bool IsOnCooldown => _onCoolDown;



        [Header("Combo")] 
        private Coroutine _comboTimer;
        private int _comboStep = 0;

        public int ComboStep
        {
            get => _comboStep;
            private set
            {
                if (_comboStep == value) return;
                _comboStep = value;
                OnComboStepChanged?.Invoke(_comboStep);
            }
        }
        private bool _isAttacking = false;
        private bool _onCoolDown = false;

        [Header("Debug")]
        [SerializeField] private bool showComboDebug = false;
        private float _comboBufferTime;
        private float _comboTimeRemaining = 0f;
        private bool _isInComboWindow = false;
        private bool _attackInputDetected = false;
        private float _inputFlashTimer = 0f;

        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerRigController rigController;

        //testing
        private bool showGizmo = false;

        private void Awake()
        {
            Model = new MeleeModel(data);
            if (!animator) animator = GetComponent<Animator>();
            if (!rigController) rigController = GetComponent<PlayerRigController>();
        }

        private void Start()
        {
            SetupStateMachineBehaviours();
        }

        private void Update()
        {
            if (_inputFlashTimer > 0f)
            {
                _inputFlashTimer -= Time.deltaTime;
                if (_inputFlashTimer <= 0f) _attackInputDetected = false;
            }
        }

        private void SetupStateMachineBehaviours()
        {
            if (animator != null)
            {
                foreach (var b in animator.GetBehaviours<FirstAttackBehaviour>()) b.SetMeleeController(this);
                foreach (var b in animator.GetBehaviours<SecondAttackBehaviour>()) b.SetMeleeController(this);
            }
        }
    
        public void Attack()
        {
            Debug.Log($"[MeleeController] Attack input, current step: {ComboStep}");

            if (!_isAttacking && !_onCoolDown && ComboStep == 0)
            {
                // Primer golpe
                ComboStep = 1;
                _isAttacking = true;
                //Debug.Log("Primer ataque");
            }
            else if (_isAttacking && ComboStep == 1)
            {
                // Segundo golpe dentro de la ventana
                ComboStep = 2;
                //Debug.Log("Segundo ataque");
                StopComboWindow();

                _attackInputDetected = true;
                _inputFlashTimer = 0.3f;
            }
        }
        public void StartComboWindow(float duration)
        {
            if (_comboTimer != null) StopCoroutine(_comboTimer);
            _comboTimer = StartCoroutine(ComboWindowCoroutine(duration));
        }

        private void StopComboWindow()
        {
            if (_comboTimer != null)
            {
                StopCoroutine(_comboTimer);
                _comboTimer = null;
            }
            _isInComboWindow = false;
        }

        private IEnumerator ComboWindowCoroutine(float duration)
        {
            _isInComboWindow = true;
            _comboTimeRemaining = duration;
            _comboBufferTime = duration;

            while (_comboTimeRemaining > 0f)
            {
                _comboTimeRemaining -= Time.deltaTime;
                yield return null;

                if (ComboStep == 2)
                {
                    //Debug.Log("Segundo ataque encadenado");
                    _isInComboWindow = false;
                    yield break;
                }
            }

            if (ComboStep == 1)
            {
                //Debug.Log("Combo perdido");
                ResetCombo();
                StartCoroutine(DoCooldown());
            }
        }
        public void ExecuteDamage()
        {
            showGizmo = true;
            Model.StartAttack(ComboStep);

            if (attackPoint != null)
            {
                Collider[] hits = Physics.OverlapSphere(attackPoint.position, Model.Range);
                foreach (Collider hit in hits)
                {
                    IDamageable damageable = hit.GetComponent<IDamageable>();
                    if (damageable != null && hit.gameObject.layer != 6) damageable.TakeDamage(Model.Damage);
                }
            }
        }

        
    
    
        public void OnAnimationComplete()
        {
            Debug.Log($"[MeleeController] Animación completa step {ComboStep}");
            showGizmo = false;

            if (ComboStep == 2)
            {
                ResetCombo();
                // Tras segundo golpe hacer cooldown
                StartCoroutine(DoCooldown());
            } 
            ResetCombo();
        }

        private void ResetCombo()
        {
            StopComboWindow();
            ComboStep = 0;
            _isAttacking = false;
            OnComboReset?.Invoke();
        }

        private IEnumerator DoCooldown()
        {
            _onCoolDown = true;
            yield return new WaitForSeconds(Model.CoolDown);
            _onCoolDown = false;

            // Disparamos evento para UI
            OnCooldownComplete?.Invoke();

            ResetCombo();
        }

        private void OnGUI()
        {
            if (!showComboDebug) return;

            GUILayout.BeginArea(new Rect(10, 150, 350, 200));
            GUILayout.Label("=== COMBO DEBUG ===", GUI.skin.box);

            GUILayout.Label($"Combo Step: {ComboStep}");
            GUILayout.Label($"IsAttacking: {_isAttacking}");
            GUILayout.Label($"Cooldown: {_onCoolDown}");

            if (_isInComboWindow)
            {
                float progress = 1f - (_comboTimeRemaining / _comboBufferTime);
                GUILayout.Label($"Combo Window: {_comboTimeRemaining:F2}s left");
                Rect bar = GUILayoutUtility.GetRect(300, 20);
                GUI.Box(bar, "");
                Rect fill = new Rect(bar.x, bar.y, bar.width * progress, bar.height);
                GUI.color = Color.Lerp(Color.green, Color.red, progress);
                GUI.DrawTexture(fill, Texture2D.whiteTexture);
                GUI.color = Color.white;
            }

            if (_attackInputDetected)
            {
                GUI.color = Color.cyan;
                GUILayout.Label("⚡ Second input detected!", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
                GUI.color = Color.white;
            }

            GUILayout.EndArea();
        }

        private void OnDrawGizmos()
        {
            if (!showGizmo || attackPoint == null) return;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, Model.Range);
        }
    }
}
