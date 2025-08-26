using UnityEngine;

public class SlashProjectile : MonoBehaviour
{

    public float speed = 10;
    public float distance = 30;
    private ParticleSystem mainParticle;

    private Vector3 initPosition;

    public void Fire()
    {
        mainParticle = GetComponent<ParticleSystem>();

        mainParticle.Play(true);

        initPosition = transform.position;
    }

    private void Update()
    {
        if (mainParticle && mainParticle.isPlaying)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, speed * Time.deltaTime);

            if (Vector3.Distance(initPosition, transform.position) > distance)
            {
                mainParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                FindAnyObjectByType<ObjectPooling>().ReturnObject(gameObject);
            }

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Collider with Player");
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                Debug.Log(".............");
                playerMovement.Dead();
            }
        }
    }
}
