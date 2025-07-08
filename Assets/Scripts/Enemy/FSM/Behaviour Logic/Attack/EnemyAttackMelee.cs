using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Melee Basic", menuName = "Enemy Logic/Attack Logic/Melee Attack")]
public class EnemyAttackMelee : EnemyAttackSOBase
{
    private bool _hasAttackedOnce = false;

    private enum AttackColorState { None, FadeIn, FadeOut }
    private AttackColorState _colorState = AttackColorState.None;

    //private bool _isAttacking = false;
    private float _attackAnimationTimer = 0f;

    [Header("Visual")]
    [SerializeField] private Color attackColor = Color.red;
    [SerializeField] private float fadeDuration = 0.5f; 


    public override void Initialize(GameObject gameObject, IEnemyBaseController enemy)
    {
        base.Initialize(gameObject, enemy);

        _enemyRenderer = gameObject.GetComponentInChildren<Renderer>();
        if (_enemyRenderer != null)
        {
            _originalColor = _enemyRenderer.material.color;
        }
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        _enemyModel.OnHealthChanged += HandleHealthChanged;
        _enemyView.OnAttackStarted += OnAttackStarted;
        _enemyView.OnAttackImpact += OnAttackImpact;

        _navMeshAgent.SetDestination(playerTransform.position);
        _hasAttackedOnce = false;
        //_isAttacking = false;
        _attackAnimationTimer = 0f;
        _colorState = AttackColorState.None;

        ResetColor();


        _navMeshAgent.isStopped = true;
        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.ResetPath();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();

        ResetColor();
        _enemyView.PlayAttackAnimation(false);

        _enemyModel.OnHealthChanged -= HandleHealthChanged;
        _enemyView.OnAttackStarted -= OnAttackStarted;
        _enemyView.OnAttackImpact -= OnAttackImpact;

        _hasAttackedOnce = false;
        //_isAttacking = false;
        _colorState = AttackColorState.None;

        _navMeshAgent.speed = _enemyModel.statsSO.moveSpeed;
        _navMeshAgent.angularSpeed = _enemyModel.statsSO.rotationSpeed;
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        if (distanceToPlayer > _distanceToCountExit)
        {
            ResetColor();
            _enemyView.PlayAttackAnimation(false);
            enemy.fsm.ChangeState(enemy.SearchState);
            return;
        }

        // === Manejo del color en degradé ===
        switch (_colorState)
        {
            case AttackColorState.FadeIn:
                _attackAnimationTimer += Time.deltaTime;
                UpdateAttackColorEffect(_attackAnimationTimer / fadeDuration);
                if (_attackAnimationTimer >= fadeDuration)
                    _attackAnimationTimer = fadeDuration;
                break;

            case AttackColorState.FadeOut:
                _attackAnimationTimer += Time.deltaTime;
                UpdateAttackColorEffect(1f - (_attackAnimationTimer / fadeDuration));
                if (_attackAnimationTimer >= fadeDuration)
                {
                    _attackAnimationTimer = 0f;
                    _colorState = AttackColorState.None;
                    ResetColor();
                }
                break;

            default:
                ResetColor();
                break;
        }

        // === Lógica de ataques ===
        if (!_hasAttackedOnce)
        {
            if (_timer >= _initialAttackDelay)
            {
                Attack();
                _hasAttackedOnce = true;
                _timer = 0f;
            }
        }
        else if (_timer >= _timeBetweenAttacks)
        {
            Attack();
            _timer = 0f;
        }

        // Rotación manual hacia el jugador
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        directionToPlayer.y = 0f; // Opcional: evita que incline la cabeza hacia arriba o abajo
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _enemyModel.statsSO.rotationSpeed * Time.deltaTime);
        }


    }

    private void Attack()
    {
        if (playerTransform == null) return;

        distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        if (distanceToPlayer <= _distanceToCountExit)
        {
            _enemyView.PlayAttackAnimation(true);
        }
    }

    private void OnAttackStarted()
    {
        _colorState = AttackColorState.FadeIn;
        _attackAnimationTimer = 0f;
    }

    private void OnAttackImpact()
    {
        _colorState = AttackColorState.FadeOut;
        _attackAnimationTimer = 0f;
    }

    private void HandleHealthChanged(float currentHealth)
    {
        if (_timer >= _initialAttackDelay)
        {
            enemy.fsm.ChangeState(enemy.StunnedState);
        }
    }

    private void UpdateAttackColorEffect(float t)
    {
        if (_enemyRenderer == null) return;

        t = Mathf.Clamp01(t);
        Color lerpedColor = Color.Lerp(_originalColor, attackColor, t);
        _enemyRenderer.material.color = lerpedColor;
    }

    private void ResetColor()
    {
        if (_enemyRenderer != null)
        {
            _enemyRenderer.material.color = _originalColor;
        }
    }
}
