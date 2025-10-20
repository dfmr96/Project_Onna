using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class EnemyView : MonoBehaviour
{

    [Header("Enemy Melee Settings")]
    [SerializeField] private Transform punchPoint;
    public Transform PunchPoint => punchPoint;
    public event Action OnAttackStarted;
    public event Action OnAttackImpact;
    public event Action OnAttackCanStun;
    public event Action OnAttackFinished;



    [Header("Pre-Attack Settings")]
    [SerializeField] private float slowAnimationFactor = 0.1f;
    [SerializeField] private float preAttackDuration = 0.6f;
    [SerializeField] private float shakeAmount = 0.09f;
    [SerializeField] private float shakeColorAtenuation = 0.2f;
    [SerializeField] private Material[] preAttackMaterials;


    private Vector3 originalPos;
    private List<Color[]> originalMaterialColors;
    Renderer[] childRenderers;
    Color[] originalColors;

    private Animator animator;
    private Transform _playerTransform;
    private EnemyController _enemyController;
    private EnemyModel _enemyModel;
    private ProjectileSpawner projectileSpawner;
    private EnemyProjectile _currentProjectile;


    //private float _distanceToCountExit = 3f;

    [Header("Spawning Settings")]
    [SerializeField] private Material[] spawnMaterials; // materiales temporales de spawn
    [SerializeField] private float spawnEffectDuration = 1f; // segundos que dura


    private Renderer targetRenderer;
    private Color flashColor = Color.white;
    private float flashDuration = .3f;
    private Material material;
    private Color originalColor;



    [Header("Torret Settings")]
    //for enemy torret
    private bool useFirstFirePoint = true;
    //Cambiar mas adelante
    public Transform firePoint2;
    public Transform turretHead;
    private Quaternion initialRotation;
    private Vector3 initialPosition;
    private float recoilAngle = 5f;
    private float recoilDistance = 2f;
    private float recoilSpeed = 5f;

    [Header("Damage Settings")]
    [SerializeField] private ParticleSystem deathParticlesPrefab;
    [SerializeField] private Material[] damageMaterials;
    [SerializeField] private Material[] deathMaterials;
    private Material[] originalMaterials;// material temporal de daño
    [SerializeField] private int[] materialIndexesToFlash = { 0 };
    [SerializeField] private ParticleSystem damageParticlesPrefab;


    private float deadAngle = 40f;
    private AudioSource audioSource;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip shootAudioClip;
    [SerializeField] private AudioClip damagedAudioClip;
    private bool isDead = false;

    private Coroutine flashCoroutine = null;
    private Coroutine recoilCoroutine;



    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        targetRenderer = GetComponentInChildren<Renderer>();
        if (targetRenderer != null)
        {
            material = targetRenderer.material;
            originalColor = material.color;
            originalMaterials = targetRenderer.materials;
        }

        childRenderers = GetComponentsInChildren<Renderer>();

        if (childRenderers.Length > 0)
        {
            originalColors = new Color[childRenderers.Length];
            for (int i = 0; i < childRenderers.Length; i++)
            {
                if (childRenderers[i].material.HasProperty("_Color"))
                    originalColors[i] = childRenderers[i].material.color;
            }
        }

        // Guardar posición y colores originales
        originalPos = transform.localPosition;

        originalMaterialColors = new List<Color[]>();

        foreach (var renderer in childRenderers)
        {
            if (renderer == null)
            {
                originalMaterialColors.Add(null);
                continue;
            }

            Material[] mats = renderer.materials;
            Color[] matColors = new Color[mats.Length];

            for (int j = 0; j < mats.Length; j++)
            {
                if (mats[j].HasProperty("_BaseColor"))
                    matColors[j] = mats[j].GetColor("_BaseColor");
                else if (mats[j].HasProperty("_Color"))
                    matColors[j] = mats[j].GetColor("_Color");
                else
                    matColors[j] = Color.white;
            }

            originalMaterialColors.Add(matColors);
        }

        projectileSpawner = GameManager.Instance.projectileSpawner;
        _enemyController = GetComponent<EnemyController>();
        _enemyModel = GetComponent<EnemyModel>();

    }

    public Animator Animator => animator;

    private void Start()
    {
      
        if (turretHead != null)
        {
            initialRotation = turretHead.localRotation;
            initialPosition = turretHead.localPosition;
        }

        _playerTransform = PlayerHelper.GetPlayer().transform;


    }

    #region Triggers
    //Animaciones
    public void PlayAttackAnimation(bool isAttacking)
    {
        animator.SetBool("IsAttacking", isAttacking);
    }

    public void PlayMeleeAttackAnimation(bool isAttacking)
    {
        animator.SetBool("IsMeleeAttacking", isAttacking);
    }

    public void PlayStrafeAnimation()
    {
        animator.SetTrigger("IsStrafing");
    }

    public bool GetBoolAttackAnimation()
    {
        return animator.GetBool("IsAttacking");
    }

    public void PlayIdleAnimation()
    {
        //animator.SetTrigger("Idle");
    }

    public void PlayMovingAnimation(float moveSpeed)
    {
        animator.SetFloat("MoveSpeed", moveSpeed);
    }

    public void PlayStunnedAnimation()
    {
        animator.SetTrigger("IsStunned");
    }

    public void HandleDamage()
    {
        animator.SetTrigger("IsDamaged");
        audioSource?.PlayOneShot(damagedAudioClip);

        // Instanciar partículas de daño si existen
        if (damageParticlesPrefab != null)
        {
            // Crear como hijo del enemy
            ParticleSystem damageParticlesInstance = Instantiate(
                damageParticlesPrefab,
                transform.position + Vector3.up * 1f,
                Quaternion.identity,
                transform
            );

            damageParticlesInstance.Play();

            // Destruir después de que termine
            Destroy(damageParticlesInstance.gameObject, 2f);
        }
    }

    //public void PlayDamageAnimation()
    //{
    //    animator.SetTrigger("IsDamaged");
    //}

    public void PlayDeathAnimation()
    {
        animator.SetTrigger("IsDead");
        isDead = true;

        // Asignamos materiales de muerte
        targetRenderer.materials = GetFittedMaterials(deathMaterials);

        // Cancelamos cualquier flash en curso
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        // Lanzamos efecto de disolver
        StartCoroutine(DissolveCoroutine(1f, 2f)); // (valor final, duración en segundos)

        PlayDeathParticles();
    }


    //public void UpdateHealthBar(float healthPercentage)
    //{
    //    //health bar logic
    //}

    #endregion

    #region Animations Functions Calls

    public void AnimationAttackStarted()
    {
        OnAttackStarted?.Invoke();
    }

    //ActionEvent de Ataque
    public void AnimationAttackFunc()
    {
        OnAttackImpact?.Invoke();

    }
    public void AnimationAttackTankFunc()
    {
        OnAttackImpact?.Invoke();
    }

    public void AnimationAttackTankMeleeFunc()
    {
        OnAttackStarted?.Invoke();
    }

    public void CanStunStarted()
    {
        OnAttackCanStun?.Invoke();

    }

    public void AnimationAttackFinished()
    {
        OnAttackFinished?.Invoke();
    }



    public void AnimationAttackPrepare()
    {
        if (isDead) return;

        //Efecto visual de advertencia
        StartCoroutine(AttackPreparationEffect());
    }

    //Evento para ataques tipo charge
    public void AnimationChargeAttackFunc()
    {
        OnAttackImpact?.Invoke();
       
    }

    public void AnimationSpawnProjectileFunc()
    {
        Transform firePoint = _enemyController.firePoint;

        Vector3 spawnPos = firePoint.position;
        Quaternion spawnRot = firePoint.rotation;

        EnemyProjectile pendingProjectile = projectileSpawner.SpawnIdleProjectile(spawnPos, spawnRot);

        _currentProjectile = pendingProjectile;

        // Aplicar el efecto visual de aparición
        var fx = pendingProjectile.GetComponent<ObjectScalerFx>();
        if (fx != null)
        {
            fx.enabled = true;
            //fx.RestartScale();
        }
    }

    public void AnimationShootProjectileFunc()
    {
        if (_currentProjectile == null) return;

        Transform firePoint = _enemyController.firePoint;

        Vector3 targetPos = _playerTransform.position;
        targetPos.y = firePoint.position.y;
        Vector3 dir = (targetPos - firePoint.position).normalized;

        _currentProjectile.Fire(dir, _enemyModel.statsSO.ShootForce, _enemyModel.currentDamage, _enemyModel);
        _currentProjectile = null;

        audioSource.PlayOneShot(shootAudioClip);
    }

    //public void AnimationShootProjectileFunc()
    //{
    //    if (projectileSpawner == null) return;

    //    Transform firePoint = _enemyController.firePoint;

    //    Vector3 targetPos = _playerTransform.position;
    //    targetPos.y = firePoint.position.y;
    //    Vector3 dir = (targetPos - firePoint.position).normalized;

    //    projectileSpawner.SpawnProjectile(firePoint.position, dir, _enemyModel.statsSO.ShootForce, _enemyModel.currentDamage, _enemyModel);

    //    audioSource.PlayOneShot(shootAudioClip);
    //}

    public void TorretShootProjectileFunc()
    {
        if (projectileSpawner == null) return;

        Transform firePoint = _enemyController.firePoint;

        Vector3 targetPos;
        Vector3 dir;

        if (useFirstFirePoint)
        {
            targetPos = _playerTransform.position;
            targetPos.y = firePoint.position.y;
            dir = (targetPos - firePoint.position).normalized;

            projectileSpawner.SpawnProjectile(firePoint.position, dir, _enemyModel.statsSO.ShootForce, _enemyModel.currentDamage, _enemyModel);
            DoRecoil();

        }
        else
        {
            targetPos = _playerTransform.position;
            targetPos.y = firePoint2.position.y;
            dir = (targetPos - firePoint2.position).normalized;

            projectileSpawner.SpawnProjectile(firePoint2.position, dir, _enemyModel.statsSO.ShootForce, _enemyModel.currentDamage, _enemyModel);
            DoRecoil();

        }

        //alterna entre firepoints
        useFirstFirePoint = !useFirstFirePoint;
    }

    #endregion

    #region Turret Visual animations
    public void DoRecoil()
    {
        if (turretHead == null) return;

        //StopAllCoroutines(); // Detener recoil anteriores

        if (recoilCoroutine != null)
            StopCoroutine(recoilCoroutine);

        StartCoroutine(RecoilCoroutine());
    }

    private IEnumerator RecoilCoroutine()
    {
        // Calculamos posici�n y rotaci�n de retroceso
        Quaternion recoilRotation = initialRotation * Quaternion.Euler(-recoilAngle, 0f, 0f);
        Vector3 recoilPosition = initialPosition - turretHead.localRotation * Vector3.forward * recoilDistance;

        // Aplicamos el retroceso instant�neo
        turretHead.localRotation = recoilRotation;
        turretHead.localPosition = recoilPosition;

        // Lerp suave hacia la posici�n y rotaci�n original
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            turretHead.localRotation = Quaternion.Slerp(recoilRotation, initialRotation, t);
            turretHead.localPosition = Vector3.Lerp(recoilPosition, initialPosition, t);
            yield return null;
        }

        // Aseguramos que termine en el lugar exacto
        turretHead.localRotation = initialRotation;
        turretHead.localPosition = initialPosition;
    }
    #endregion

    #region Visual Effects
    public void PlayDeathParticles()
    {
        if (deathParticlesPrefab != null)
        {
            ParticleSystem deathParticlesInstance = Instantiate(deathParticlesPrefab, transform.position + new Vector3(0,1,0), Quaternion.identity);
            deathParticlesInstance.Play();

            if (turretHead != null)
                turretHead.localRotation = Quaternion.Euler(deadAngle, 0f, 0f);

            Destroy(deathParticlesInstance.gameObject, 2f);
        }
    }


    public void PlaySpawnEffect()
    {
        if (isDead) return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        // Lanzamos la corutina del spawn dissolve
        flashCoroutine = StartCoroutine(SpawnDissolveCoroutine(1f, 0f, spawnEffectDuration));
    }


    private IEnumerator SpawnDissolveCoroutine(float startValue, float endValue, float duration)
    {
        // Aplicamos el material de spawn
        targetRenderer.materials = GetFittedMaterials(spawnMaterials);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float dissolveValue = Mathf.Lerp(startValue, endValue, t);

            foreach (Material mat in targetRenderer.materials)
            {
                if (mat.HasProperty("_DissolveAmount"))
                    mat.SetFloat("_DissolveAmount", dissolveValue);
            }

            yield return null;
        }

        // Restauramos materiales originales
        if (!isDead)
            targetRenderer.materials = originalMaterials;

        flashCoroutine = null;
    }

    private IEnumerator DissolveCoroutine(float targetValue, float duration)
    {
        float elapsed = 0f;

        // Asumimos que todos los materiales de muerte tienen la propiedad DissolveAmount
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            foreach (Material mat in targetRenderer.materials)
            {
                if (mat.HasProperty("_DissolveAmount"))
                    mat.SetFloat("_DissolveAmount", Mathf.Lerp(0f, targetValue, t));
            }

            yield return null;
        }

        // Aseguramos valor final
        foreach (Material mat in targetRenderer.materials)
        {
            if (mat.HasProperty("_DissolveAmount"))
                mat.SetFloat("_DissolveAmount", targetValue);
        }
    }

    public void PlayDamageEffect()
    {
        if (isDead) return; // si está muerto no hacemos flash

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashDamageColorsCoroutine());
    }

    private IEnumerator FlashDamageColorsCoroutine()
    {
        // Guardar los colores originales de los materiales actuales
        Color[] originalColors = new Color[targetRenderer.materials.Length];
        for (int i = 0; i < targetRenderer.materials.Length; i++)
        {
            Material mat = targetRenderer.materials[i];
            if (mat.HasProperty("_BaseColor"))
                originalColors[i] = mat.GetColor("_BaseColor");
            else if (mat.HasProperty("_Color"))
                originalColors[i] = mat.GetColor("_Color");
        }

        // Aplicar el color de daño (flash)
        for (int i = 0; i < targetRenderer.materials.Length; i++)
        {
            Material mat = targetRenderer.materials[i];
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", Color.white); // ejemplo de flash
            else if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", Color.white);
        }

        yield return new WaitForSeconds(flashDuration);

        // Restaurar colores originales de forma segura
        for (int i = 0; i < targetRenderer.materials.Length; i++)
        {
            Material mat = targetRenderer.materials[i];
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", originalColors[i]);
            else if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", originalColors[i]);
        }

        flashCoroutine = null;
    }

   
    private Material[] GetFittedMaterials(Material[] source)
    {
        Material[] newMaterials = new Material[originalMaterials.Length];

        for (int i = 0; i < originalMaterials.Length; i++)
        {
            if (i < source.Length)
                newMaterials[i] = source[i];
            else
                newMaterials[i] = source[source.Length - 1]; // repetir el último si faltan
        }

        return newMaterials;
    }

    private IEnumerator AttackPreparationEffect()
    {
        float originalSpeed = animator.speed;
        animator.speed = slowAnimationFactor;

        Vector3 originalPos = transform.localPosition;

        // Guardamos materiales originales por renderer
        List<Material[]> originalMaterialsPerRenderer = new List<Material[]>();
        foreach (var r in childRenderers)
            originalMaterialsPerRenderer.Add(r != null ? r.materials : null);

        float elapsed = 0f;

        try
        {
            while (elapsed < preAttackDuration)
            {
                elapsed += Time.deltaTime;
                transform.localPosition = originalPos + UnityEngine.Random.insideUnitSphere * shakeAmount;

                float normalizedTime = Mathf.Clamp01(elapsed / preAttackDuration);

                // Crescendo lineal: 0 al inicio, 1 al final
                float crescendo = normalizedTime * shakeColorAtenuation; 

                // Aplicamos interpolación de colores
                for (int i = 0; i < childRenderers.Length; i++)
                {
                    Renderer r = childRenderers[i];
                    if (r == null) continue;

                    Material[] mats = r.materials;
                    Color[] baseColors = originalMaterialColors[i];

                    for (int j = 0; j < mats.Length; j++)
                    {
                        if (mats[j] == null) continue;

                        Color baseColor = baseColors[j];
                        Color targetColor = Color.Lerp(baseColor, Color.white, crescendo); // blanco como "prendido"

                        if (mats[j].HasProperty("_BaseColor"))
                            mats[j].SetColor("_BaseColor", targetColor);
                        else if (mats[j].HasProperty("_Color"))
                            mats[j].SetColor("_Color", targetColor);

                        if (mats[j].HasProperty("_EmissionColor"))
                        {
                            Color emission = Color.Lerp(Color.black, Color.white, crescendo);
                            mats[j].SetColor("_EmissionColor", emission);
                            mats[j].EnableKeyword("_EMISSION");
                        }
                    }
                }

                yield return null;
            }
        }
        finally
        {
            animator.speed = originalSpeed;
            //transform.localPosition = originalPos;
            RestoreOriginalColorsAndPosition();
            // Restauramos materiales originales
            //for (int i = 0; i < childRenderers.Length; i++)
            //{
            //    Renderer r = childRenderers[i];
            //    if (r == null) continue;
            //    r.materials = originalMaterialsPerRenderer[i];
            //}
        }
    }



    public void RestoreOriginalColorsAndPosition(bool restorePosition = false)
    {
   
        
        // Restaurar posición
        if (restorePosition)
            transform.localPosition = originalPos;

        if (childRenderers == null || originalMaterialColors == null)
            return;

        for (int i = 0; i < childRenderers.Length; i++)
        {
            Renderer r = childRenderers[i];
            if (r == null) continue;

            Material[] mats = r.materials;
            Color[] originalColors = originalMaterialColors[i];
            if (originalColors == null) continue;

            for (int j = 0; j < mats.Length; j++)
            {
                if (mats[j] == null) continue;
                Color c = originalColors[j];

                // Restaurar color base
                if (mats[j].HasProperty("_BaseColor"))
                    mats[j].SetColor("_BaseColor", c);
                else if (mats[j].HasProperty("_Color"))
                    mats[j].SetColor("_Color", c);

                // Restaurar emisión
                if (mats[j].HasProperty("_EmissionColor"))
                {
                    mats[j].SetColor("_EmissionColor", Color.black);
                    mats[j].DisableKeyword("_EMISSION");
                }
            }
        }
    }

    #endregion

    //Gizmos de tamano de golpe
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(punchPoint.position, 1f);
    }
}

