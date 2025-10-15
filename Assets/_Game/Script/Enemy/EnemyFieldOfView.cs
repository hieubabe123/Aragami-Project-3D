using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFieldOfView : MonoBehaviour
{
    [Header("Detection FOV")]
    public float detectionRadius = 15f;
    [Range(0, 360)]
    public float detectionAngle = 90f;

    [Header("Attack FOV")]
    public float attackRadius = 5f;
    [Range(0, 360)]
    public float attackAngle = 60f;

    [Header("Masks")]
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    [SerializeField] private float targetHeightOffset = 1.2f;

    [Header("Performance Settings")]
    [SerializeField] private float scanFrequency = 0.1f; // Quét mỗi 0.1 giây thay vì mỗi frame
    [SerializeField] private int maxTargetsToCheck = 10; // Giới hạn số target check mỗi lần

    // Danh sách các mục tiêu trong tầm nhìn
    [HideInInspector]
    public List<Transform> visibleDetectionTargets = new List<Transform>();

    [HideInInspector]
    public List<Transform> visibleAttackTargets = new List<Transform>();

    // Cache để tối ưu performance
    private Collider[] _targetBuffer;
    private List<Transform> _tempDetectionList = new List<Transform>();
    private List<Transform> _tempAttackList = new List<Transform>();
    private Coroutine _scanCoroutine;
    private Vector3 _lastPosition;
    private Quaternion _lastRotation;

    private void Start()
    {
        // Khởi tạo buffer với kích thước phù hợp
        _targetBuffer = new Collider[maxTargetsToCheck];

        // Bắt đầu coroutine scan
        _scanCoroutine = StartCoroutine(ScanForTargets());

        _lastPosition = transform.position;
        _lastRotation = transform.rotation;
    }

    private void OnEnable()
    {
        if (_scanCoroutine == null)
        {
            _scanCoroutine = StartCoroutine(ScanForTargets());
        }
    }

    private void OnDisable()
    {
        if (_scanCoroutine != null)
        {
            StopCoroutine(_scanCoroutine);
            _scanCoroutine = null;
        }
    }

    private IEnumerator ScanForTargets()
    {
        while (true)
        {
            FindVisibleTargets();
            yield return new WaitForSeconds(scanFrequency);
        }
    }

    private void FindVisibleTargets()
    {
        _tempDetectionList.Clear();
        _tempAttackList.Clear();

        // Tìm mục tiêu trong vùng PHÁT HIỆN trước (vì radius lớn hơn)
        FindTargetsInFOV(detectionRadius, detectionAngle, _tempDetectionList);

        // Với attack, chỉ cần check những target đã có trong detection
        // (vì attack radius < detection radius)
        for (int i = 0; i < _tempDetectionList.Count; i++)
        {
            Transform target = _tempDetectionList[i];
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget <= attackRadius)
            {
                Vector3 targetPoint = GetTargetPoint(target);
                Vector3 dirToTarget = (targetPoint - transform.position).normalized;

                if (Vector3.Angle(transform.forward, dirToTarget) < attackAngle / 2)
                {
                    _tempAttackList.Add(target);
                }
            }
        }

        // Cập nhật danh sách chính
        visibleDetectionTargets.Clear();
        visibleDetectionTargets.AddRange(_tempDetectionList);

        visibleAttackTargets.Clear();
        visibleAttackTargets.AddRange(_tempAttackList);
    }

    private void FindTargetsInFOV(float radius, float angle, List<Transform> visibleList)
    {

        // Sử dụng OverlapSphereNonAlloc để tránh garbage collection
        int numTargets = Physics.OverlapSphereNonAlloc(transform.position, radius, _targetBuffer, targetMask);

        for (int i = 0; i < numTargets && i < maxTargetsToCheck; i++)
        {
            Transform target = _targetBuffer[i].transform;

            // Tránh detect chính bản thân
            if (target.gameObject == gameObject)
                continue;

            Vector3 targetPoint = GetTargetPoint(target);
            Vector3 dirToTarget = (targetPoint - transform.position).normalized;

            // Kiểm tra góc nhìn
            float angleToTarget = Vector3.Angle(transform.forward, dirToTarget);
            if (angleToTarget > angle / 2)
                continue;

            // Kiểm tra vật cản
            float distanceToTarget = Vector3.Distance(transform.position, targetPoint);

            // Sử dụng SphereCast thay vì Raycast để detection tốt hơn
            if (!Physics.SphereCast(transform.position, 0.1f, dirToTarget, out RaycastHit hit, distanceToTarget, obstacleMask))
            {
                visibleList.Add(target);
            }
            else
            {
                // Kiểm tra xem raycast có hit đúng target không (trường hợp target là obstacle)
                if (hit.transform == target || hit.transform.IsChildOf(target))
                {
                    visibleList.Add(target);
                }
            }
        }
    }

    private Vector3 GetTargetPoint(Transform target)
    {
        // Tìm AimTargetPoint trước
        Transform aimTarget = target.Find("AimTargetPoint");
        if (aimTarget != null)
        {
            return aimTarget.position;
        }

        // Thử tìm trong children (trường hợp AimTargetPoint là nested)
        aimTarget = target.GetComponentInChildren<Transform>();
        if (aimTarget != null && aimTarget.name == "AimTargetPoint")
        {
            return aimTarget.position;
        }

        // Fallback về center + offset
        Collider targetCollider = target.GetComponent<Collider>();
        if (targetCollider != null)
        {
            return targetCollider.bounds.center;
        }

        return target.position + Vector3.up * targetHeightOffset;
    }

    // Method để force scan ngay lập tức (có thể gọi từ EnemyState khi cần)
    public void ForceScan()
    {
        FindVisibleTargets();
    }

    // Method để check xem có target nào trong tầm không (không cần list)
    public bool HasTargetInDetection()
    {
        return visibleDetectionTargets.Count > 0;
    }

    public bool HasTargetInAttackRange()
    {
        return visibleAttackTargets.Count > 0;
    }

    // Lấy target gần nhất trong detection range
    public Transform GetClosestDetectionTarget()
    {
        if (visibleDetectionTargets.Count == 0)
            return null;

        Transform closest = visibleDetectionTargets[0];
        float closestDistance = Vector3.Distance(transform.position, closest.position);

        for (int i = 1; i < visibleDetectionTargets.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, visibleDetectionTargets[i].position);
            if (distance < closestDistance)
            {
                closest = visibleDetectionTargets[i];
                closestDistance = distance;
            }
        }

        return closest;
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    // Debug drawing (chỉ trong Scene view)
    private void OnDrawGizmosSelected()
    {
        // Draw detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw attack radius  
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        // Draw detection FOV
        Vector3 viewAngleA = DirectionFromAngle(-detectionAngle / 2, false);
        Vector3 viewAngleB = DirectionFromAngle(detectionAngle / 2, false);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * detectionRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * detectionRadius);

        // Draw attack FOV
        Vector3 attackAngleA = DirectionFromAngle(-attackAngle / 2, false);
        Vector3 attackAngleB = DirectionFromAngle(attackAngle / 2, false);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + attackAngleA * attackRadius);
        Gizmos.DrawLine(transform.position, transform.position + attackAngleB * attackRadius);

        // Draw lines to visible targets
        Gizmos.color = Color.green;
        foreach (Transform visibleTarget in visibleDetectionTargets)
        {
            if (visibleTarget != null)
                Gizmos.DrawLine(transform.position, GetTargetPoint(visibleTarget));
        }
    }
}