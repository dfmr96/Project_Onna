using Mutations;
using Mutations.Core;
using System;
using UnityEngine;
using Player;
using System.Collections.Generic;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class Bullet : MonoBehaviour
{
    private float _bulletSpeed;
    private float _damage;
    private float _maxDistance;
    private Material[] defaultTrailMaterials;


    private GameObject playerGO;

    [SerializeField] private GameObject hitParticlePrefab;

    [SerializeField] private LayerMask ignoreLayers;
    [SerializeField] private LayerMask obstacleLayers; // Lo que normalmente impacta
    private bool ignoreObstacles = false; // Activado por Gamma Minor Muscular

    private List<BulletModifierSO> _modifiers = new List<BulletModifierSO>();
    private PlayerControllerEffect _playerEffect;


    private int _maxPenetration = 1;  // por defecto 1 (impacta y se destruye)
    private int _currentPenetration = 0;

    private void Start()
    {
        float destroyTime = _maxDistance / _bulletSpeed;
        Destroy(gameObject, destroyTime);
        playerGO = PlayerHelper.GetPlayer();

        //// Llamar OnSetup de cada modificador
        //foreach (var mod in _modifiers)
        //    mod.OnSetup(this, playerGO.GetComponent<PlayerControllerEffect>());

        var trail = GetComponentInChildren<TrailRenderer>();
        if (trail != null)
            defaultTrailMaterials = trail.materials;

        
    }

    public void RegisterModifier(BulletModifierSO modifier, PlayerControllerEffect player)
    {
        if (!_modifiers.Contains(modifier))
        {
            _modifiers.Add(modifier);
            Debug.Log("[Bullet] Modifier registrado: " + modifier.name);

        }
        _playerEffect = player;
    }

    public void SetIgnoreObstacles(bool value)
    {
        ignoreObstacles = value;
    }

    private void Update() { Move(); }

    private void Move() { transform.Translate(Vector3.forward * (_bulletSpeed * Time.deltaTime)); }

    public void Setup(float speed, float distance, float damage, PlayerModel _playerModel)
    {
        _bulletSpeed = speed;
        _maxDistance = distance;
        this._damage = damage;
        _maxPenetration = _playerModel.BulletMaxPenetration;

        Debug.Log("Cantidad de Penetracion: " + _maxPenetration);

    }

    private void OnTriggerEnter(Collider other)
    {
        int layer = 1 << other.gameObject.layer;
        Debug.Log($"Bullet collided with {other.name} layer {other.gameObject.layer} | ignoreObstacles={ignoreObstacles}");


        if ((ignoreLayers.value & (1 << other.gameObject.layer)) != 0)
            return;

        // Ignorar obstáculos si la mutación dice que sí
        if (ignoreObstacles && (obstacleLayers.value & layer) != 0)
            return;

        // instanciar partículas siempre al colisionar
        if (hitParticlePrefab != null)
        {
            GameObject hitVFX = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
            Destroy(hitVFX, 2f); // se destruye a los 2 segundos
        }


        if (other.TryGetComponent<IDamageable>(out var damageable))
            damageable.TakeDamage(_damage);

   

        //MUTACIONES DE BALAS
        foreach (var mod in _modifiers)
            mod.OnHit(this, other.gameObject, _playerEffect);

        //Penetracion
        _currentPenetration++;
        if (_currentPenetration >= _maxPenetration)
            Destroy(gameObject);
    }

    public void ApplyTrailMaterials()
    {
        var trail = GetComponentInChildren<TrailRenderer>();
        if (trail == null) return;

        List<Material> materials = new List<Material>();

        foreach (var mod in _modifiers)
        {
            Material mat = mod.GetTrailMaterial();
            if (mat != null)
                materials.Add(mat);
        }

        if (materials.Count == 0)
        {
            // Si no hay mods, usar el material por defecto
            if (defaultTrailMaterials != null)
                trail.materials = defaultTrailMaterials;
            else
                Debug.LogWarning("[Bullet] No hay materiales para aplicar ni predeterminados.");
        }
        else
        {
            trail.materials = materials.ToArray();
        }
    }





}