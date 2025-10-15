using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Transform _playerTransform;

    [Header("Patrol Position")]
    public Transform[] patrolPoints;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolWaitTime = 2f;

    private int currentPatrolIndex = 0;
    public bool isWaiting = false;
    private Coroutine _waitCoroutine;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.autoBraking = true;
        _agent.updateRotation = true;
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
        {
            _playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (!_agent.enabled)
        {
            return;
        }

        // Chỉ kiểm tra khi có điểm tuần tra và không đang trong trạng thái chờ
        if (patrolPoints.Length > 0 && !isWaiting)
        {
            // Nếu đã đến gần điểm đến, bắt đầu coroutine chờ và xoay
            if (!_agent.pathPending && _agent.remainingDistance < 0.5f && _agent.hasPath)
            {
                if (_waitCoroutine == null) // Tránh start multiple coroutines
                {
                    _waitCoroutine = StartCoroutine(WaitAndRotateAtPoint());
                }
            }
        }
    }

    // Coroutine này chỉ xử lý việc chờ và xoay tại MỘT điểm
    private IEnumerator WaitAndRotateAtPoint()
    {
        isWaiting = true;
        _agent.isStopped = true;

        yield return new WaitForSeconds(patrolWaitTime);

        if (!_agent.enabled)
        {
            isWaiting = false;
            _waitCoroutine = null;
            yield break;
        }

        // Di chuyển đến điểm tiếp theo
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        Transform nextTargetPoint = patrolPoints[currentPatrolIndex];

        _agent.isStopped = false;
        if (_agent.isOnNavMesh)
        {
            _agent.destination = nextTargetPoint.position;
        }

        isWaiting = false;
        _waitCoroutine = null;
    }

    #region Public Methods for EnemyState
    public void StartPatrolling()
    {
        if (!_agent.enabled) return;

        // Cleanup coroutine cũ
        if (_waitCoroutine != null)
        {
            StopCoroutine(_waitCoroutine);
            _waitCoroutine = null;
        }

        if (patrolPoints.Length > 0)
        {
            currentPatrolIndex = 0;
            _agent.isStopped = false;
            _agent.updateRotation = true;
            isWaiting = false;

            if (_agent.isOnNavMesh)
            {
                _agent.destination = patrolPoints[currentPatrolIndex].position;
            }
        }
        Debug.Log($"Starting patrol with {patrolPoints.Length} points. Agent enabled: {_agent.enabled}, OnNavMesh: {_agent.isOnNavMesh}");

    }

    public void ChaseTarget(Transform target)
    {
        if (!_agent.enabled || target == null) return;

        // Cleanup patrol coroutine
        if (_waitCoroutine != null)
        {
            StopCoroutine(_waitCoroutine);
            _waitCoroutine = null;
        }

        isWaiting = false;
        _agent.isStopped = false;
        _agent.updateRotation = false; // EnemyState sẽ handle rotation

        if (_agent.isOnNavMesh)
        {
            _agent.destination = target.position;
        }
    }

    public void StopMovement()
    {
        if (!_agent.enabled) return;

        // Cleanup coroutine
        if (_waitCoroutine != null)
        {
            StopCoroutine(_waitCoroutine);
            _waitCoroutine = null;
        }

        isWaiting = true; // Set waiting để animation biết
        _agent.isStopped = true;

        if (_agent.isOnNavMesh)
        {
            _agent.destination = transform.position;
        }
    }
    #endregion

    public void SetNavMeshDisable()
    {
        // Cleanup trước khi disable
        if (_waitCoroutine != null)
        {
            StopCoroutine(_waitCoroutine);
            _waitCoroutine = null;
        }

        if (_agent.enabled)
        {
            _agent.enabled = false;
        }
    }

    public IEnumerator SetRotationFacingTargetCoroutine()
    {
        if (_playerTransform == null) yield break;

        Vector3 direction = (_playerTransform.position - transform.position);
        direction.y = 0; // Chỉ xoay theo trục Y

        if (direction == Vector3.zero) yield break;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        while (Quaternion.Angle(transform.rotation, lookRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
            yield return null;
        }
    }

    // Getter methods cho debugging và animation
    public bool IsMoving()
    {
        return _agent.enabled && !_agent.isStopped && _agent.velocity.magnitude > 0.1f;
    }

    public float GetCurrentSpeed()
    {
        return _agent.enabled ? _agent.velocity.magnitude : 0f;
    }
}