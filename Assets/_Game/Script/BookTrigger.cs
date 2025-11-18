using UnityEngine;

public class BookTrigger : MonoBehaviour
{
    [SerializeField] private float speedRotate = 5f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            this.gameObject.SetActive(false);
            EventDispatcher.Dispatch<EventDefine.OnPlayerPickBook>();
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * speedRotate * Time.deltaTime);
    }
}
