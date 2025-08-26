using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private ObjectPooling _slashProjectilePool;
    [SerializeField] private Transform attackPosition;

    void Awake()
    {
        _slashProjectilePool = GetComponent<ObjectPooling>();
    }

    public void SpawnProjectile()
    {
        GameObject slashProjectile = _slashProjectilePool.GetObject();
        slashProjectile.transform.position = attackPosition.position;
        slashProjectile.transform.rotation = attackPosition.rotation;
        slashProjectile.GetComponent<SlashProjectile>().Fire();
    }

}
