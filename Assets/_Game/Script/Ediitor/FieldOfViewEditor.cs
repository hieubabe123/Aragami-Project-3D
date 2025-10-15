using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyFieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyFieldOfView fow = (EnemyFieldOfView)target;

        // --- Vẽ vùng nhìn PHÁT HIỆN (màu trắng) ---
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.detectionRadius);
        Vector3 viewAngleA = fow.DirectionFromAngle(-fow.detectionAngle / 2, false);
        Vector3 viewAngleB = fow.DirectionFromAngle(fow.detectionAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.detectionRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.detectionRadius);

        // Vẽ đường tới mục tiêu trong vùng phát hiện (màu xanh)
        Handles.color = Color.green;
        foreach (Transform visibleTarget in fow.visibleDetectionTargets)
        {
            Handles.DrawLine(fow.transform.position, visibleTarget.position);
        }

        // --- Vẽ vùng nhìn TẤN CÔNG (màu đỏ) ---
        Handles.color = Color.red;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.attackRadius);
        Vector3 attackAngleA = fow.DirectionFromAngle(-fow.attackAngle / 2, false);
        Vector3 attackAngleB = fow.DirectionFromAngle(fow.attackAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + attackAngleA * fow.attackRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + attackAngleB * fow.attackRadius);

        // Vẽ đường tới mục tiêu trong vùng tấn công (màu đỏ đậm)
        foreach (Transform visibleTarget in fow.visibleAttackTargets)
        {
            Handles.DrawLine(fow.transform.position, visibleTarget.position);
        }
    }
}