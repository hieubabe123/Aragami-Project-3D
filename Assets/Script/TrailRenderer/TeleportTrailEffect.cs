using System.Collections;
using UnityEngine;

public class TeleportTrailEffect : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 70f;
    private TrailRenderer _trailRenderer;
    private void Awake()
    {
        _trailRenderer = GetComponentInChildren<TrailRenderer>();
    }

    public IEnumerator PlayTrail(Vector3 startPosition, Vector3 targetPosition)
    {
        transform.position = startPosition;
        _trailRenderer.Clear();
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        gameObject.SetActive(false);

    }
}
