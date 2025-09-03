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
    private EnemyState _enemyState;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _enemyMovement = GetComponent<EnemyMovement>();
        _enemyState = GetComponent<EnemyState>();
    }

    private void Update()
    {
        CheckWalkingState();
    }

    private void CheckWalkingState()
    {
        // Sử dụng cả thông tin từ movement và state để quyết định animation
        bool shouldWalk = false;

        if (_enemyState != null)
        {
            // Lấy state hiện tại để kiểm tra chính xác
            EnemyStateTypes currentState = _enemyState.GetCurrentState();

            // Chỉ walking khi:
            // 1. Đang trong state Patrolling hoặc Chasing
            // 2. Movement không đang waiting
            // 3. NavMeshAgent đang di chuyển (có velocity)
            shouldWalk = (currentState == EnemyStateTypes.Patrolling || currentState == EnemyStateTypes.Chasing)
                        && !_enemyMovement.isWaiting
                        && _enemyMovement.GetComponent<UnityEngine.AI.NavMeshAgent>().velocity.magnitude > 0.1f;
        }
        else
        {
            // Fallback về cách cũ nếu không có EnemyState
            shouldWalk = !_enemyMovement.isWaiting;
        }

        _animator.SetBool(ISWALKING, shouldWalk);
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