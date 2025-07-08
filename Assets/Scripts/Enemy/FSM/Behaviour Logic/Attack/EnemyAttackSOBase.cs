using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using UnityEngine.AI;

public class EnemyAttackSOBase : ScriptableObject
{
    protected EnemyController enemy;
    protected EnemyModel _enemyModel;
    protected EnemyView _enemyView;
    protected Transform transform;
    protected GameObject gameObject;

    protected Transform playerTransform;

    protected NavMeshAgent _navMeshAgent;
    protected float initialSpeed;

    [Header("Attacking Settings")]
    [SerializeField] public float _distanceToCountExit = 3f;
    [SerializeField] protected float AttackingMovingSpeed;
    [SerializeField] protected bool isMovingSpeedChangesOnAttack;
    [SerializeField] protected float _timeBetweenAttacks = 1.5f;
    [SerializeField] protected float _initialAttackDelay = 0.3f;
    [SerializeField] protected Color _targetColor = Color.red;
    [SerializeField] protected bool isLookingPlayer = true;
    [SerializeField] protected float rotationSpeed = 5f;



    protected float distanceToPlayer;
    protected float _timer;


    //InitialAttackDelay Visual
    protected Material _material;
    protected Color _originalColor;

    protected float _colorChangeTimer = 0f;
    protected float _colorTransitionDuration;
    protected enum ColorPhase { None, ToRed, ToOriginal }
    protected ColorPhase _colorPhase = ColorPhase.None;
    public virtual void Initialize(GameObject gameObject, EnemyController enemy)
    {
        this.gameObject = gameObject;
        this.enemy = enemy;
        transform = gameObject.transform;

        playerTransform = PlayerHelper.GetPlayer().transform;
        _navMeshAgent = enemy.GetComponent<NavMeshAgent>();


    }

    public virtual void DoEnterLogic()
    {
        _timer = 0f;

        initialSpeed = _navMeshAgent.speed;

        
        if(isMovingSpeedChangesOnAttack)
        {
            _navMeshAgent.speed = AttackingMovingSpeed;

        }

        _enemyModel = enemy.GetComponent<EnemyModel>();
        _enemyView = enemy.GetComponent<EnemyView>();

        initialSpeed = _navMeshAgent.speed;
        _navMeshAgent.speed = 0;
        _navMeshAgent.isStopped = true;

        //InitialAttackDelay Visual
        _colorTransitionDuration = _initialAttackDelay;
        _material = enemy.GetComponentInChildren<Renderer>().material;
        _originalColor = _material.color;
    }
    public virtual void DoExitLogic() { ResetValues(); }

    public virtual void DoFrameUpdateLogic()
    {
        //Siempre mira al Player al atacar
        if(isLookingPlayer)
        {
            //enemy.transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));

            Vector3 direction = new Vector3(
                    playerTransform.position.x,
                    transform.position.y,
                    playerTransform.position.z
                    ) - enemy.transform.position;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemy.transform.rotation = Quaternion.Slerp(
                    enemy.transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }

        }

        ColorChanger();

        _timer += Time.deltaTime;

        //Si el Player muere durante el atque el enemigo se pone en idle
        if (playerTransform == null)
        {

            enemy.fsm.ChangeState(enemy.IdleState);
            return;
        }



    }
    public virtual void ResetValues()
    {

        _navMeshAgent.speed = initialSpeed;
        _navMeshAgent.isStopped = false;

        if (_material != null)
            _material.color = _originalColor;

        _colorPhase = ColorPhase.None;
        _colorChangeTimer = 0f;
        _timer = 0f;

    }

    private void ColorChanger()
    {
        if (_colorPhase != ColorPhase.None)
        {
            _colorChangeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(_colorChangeTimer / _colorTransitionDuration);

            if (_colorPhase == ColorPhase.ToRed)
            {
                _material.color = Color.Lerp(_originalColor, _targetColor, t);

                if (t >= 1f)
                {
                    //Vuelve al color original
                    _colorChangeTimer = 0f;
                    _colorPhase = ColorPhase.ToOriginal;
                }
            }
            else if (_colorPhase == ColorPhase.ToOriginal)
            {
                _material.color = Color.Lerp(_targetColor, _originalColor, t);

                if (t >= 1f)
                {   //Efecto terminado
                    _colorPhase = ColorPhase.None;
                }
            }
        }
    }

    protected void TriggerAttackColorEffect()
    {
        _colorChangeTimer = 0f;
        _colorPhase = ColorPhase.ToRed;
    }
}
