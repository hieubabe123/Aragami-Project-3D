using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent _agent;

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
    }

    // Thêm hàm Update() để kiểm tra liên tục
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
            if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            {
                _waitCoroutine = StartCoroutine(WaitAndRotateAtPoint());
            }
        }
    }

    // Coroutine này BÂY GIỜ CHỈ xử lý việc chờ và xoay tại MỘT điểm
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
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        Transform nextTargetPoint = patrolPoints[currentPatrolIndex];

        _agent.isStopped = false;
        _agent.destination = nextTargetPoint.position;

        isWaiting = false;
        _waitCoroutine = null;
    }
    #region Public Methods for EnemyState
    public void StartPatrolling()
    {
        if (!_agent.enabled) return;
        if (patrolPoints.Length > 0)
        {
            currentPatrolIndex = 0;
            _agent.isStopped = false;
            _agent.updateRotation = true;
            _agent.destination = patrolPoints[currentPatrolIndex].position;
            isWaiting = false;
        }
    }

    public void ChaseTarget(Transform target)
    {
        if (!_agent.enabled) return;
        if (_waitCoroutine != null)
        {
            StopCoroutine(_waitCoroutine);
            _waitCoroutine = null;
            isWaiting = false;
        }
        _agent.isStopped = false;
        _agent.updateRotation = false;
        _agent.destination = target.position;
    }

    public void StopMovement()
    {
        if (!_agent.enabled) return;

        if (_waitCoroutine != null)
        {
            StopCoroutine(_waitCoroutine);
            _waitCoroutine = null;
            isWaiting = true;
        }
        _agent.isStopped = true;
        if (_agent.isOnNavMesh) _agent.destination = transform.position;
    }
    #endregion

    public void SetNavMeshDisable()
    {
        if (_agent.enabled)
        {
            _agent.enabled = false;
        }
    }

    public IEnumerator SetRotationFacingTargetCoroutine()
    {
        Transform target = FindFirstObjectByType<PlayerMovement>().transform;
        if (target == null) yield break;

        Vector3 direction = (target.position - transform.position);
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        while (Quaternion.Angle(transform.rotation, lookRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
            yield return null;
        }
    }
}