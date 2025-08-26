using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private const string ATTACK = "Attack";
    private const string ISWALKING = "IsWalking";
    private const string DEAD = "Dead";
    private const string DEAD_ID = "DeadID";
    private const string ALERT_ALL_ENEMY = "AlertAllEnemy";
    private Animator _animator;
    private EnemyMovement _enemyMovement;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _enemyMovement = GetComponent<EnemyMovement>();
    }
    private void Update()
    {
        CheckWalkingState();
    }

    private void CheckWalkingState()
    {
        _animator.SetBool(ISWALKING, !_enemyMovement.isWaiting);
    }

    public void ExecuteAttackAnimation()
    {
        _animator.SetTrigger(ATTACK);
    }

    public void ExecuteDeadAnimation(float deadID)
    {

        if (deadID == 2 || deadID == 3)
        {
            StartCoroutine(_enemyMovement.SetRotationFacingTargetCoroutine());
        }
        _animator.SetTrigger(DEAD);
        _animator.SetFloat(DEAD_ID, deadID);
    }

    public void ExecuteAlertAllEnemyAnimation()
    {
        _animator.SetTrigger(ALERT_ALL_ENEMY);
    }
}
