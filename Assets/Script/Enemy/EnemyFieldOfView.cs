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

    // Danh sách các mục tiêu trong tầm nhìn
    [HideInInspector]
    public List<Transform> visibleDetectionTargets = new List<Transform>();

    [HideInInspector]
    public List<Transform> visibleAttackTargets = new List<Transform>();

    private void Update()
    {
        // Cập nhật cả hai vùng nhìn mỗi frame
        FindVisibleTargets();
    }

    private void FindVisibleTargets()
    {
        visibleDetectionTargets.Clear();
        visibleAttackTargets.Clear();

        // Tìm mục tiêu trong vùng PHÁT HIỆN
        FindTargetsInFOV(detectionRadius, detectionAngle, visibleDetectionTargets);

        // Tìm mục tiêu trong vùng TẤN CÔNG
        FindTargetsInFOV(attackRadius, attackAngle, visibleAttackTargets);
    }

    private void FindTargetsInFOV(float radius, float angle, List<Transform> visibleList)
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, radius, targetMask);
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Transform aimTarget = target.Find("AimTargetPoint");
            Vector3 targetPoint;
            if (aimTarget != null)
            {
                targetPoint = aimTarget.position;
            }
            else
            {
                targetPoint = target.position + Vector3.up * targetHeightOffset;
            }
            Vector3 dirToTarget = (targetPoint - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetPoint);
                if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
                {
                    visibleList.Add(target);
                }
            }
        }
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}