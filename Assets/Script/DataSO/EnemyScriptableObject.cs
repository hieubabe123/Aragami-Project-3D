using UnityEngine;

[CreateAssetMenu(fileName = "Data SO", menuName = "Data/ Enemy Data")]
public class EnemyScriptableObject : ScriptableObject
{
    [SerializeField] private float suspectDuration;
    public float SuspectDuration { get => suspectDuration; private set => suspectDuration = value; }


    [SerializeField] private float alertDuration;
    public float AlertDuration { get => alertDuration; private set => alertDuration = value; }


    [SerializeField] private float attackCooldown;
    public float AttackCooldown { get => attackCooldown; private set => attackCooldown = value; }


    [SerializeField] private float searchDuration;
    public float SearchDuration { get => searchDuration; private set => searchDuration = value; }


    [SerializeField] private float rotationSpeed;
    public float RotationSpeed { get => rotationSpeed; private set => rotationSpeed = value; }


    [SerializeField] private float attackLeash;
    public float AttackLeash { get => attackLeash; private set => attackLeash = value; }


    [SerializeField] private bool isAlertedAllEnemy;
    public bool IsAlertedAllEnemy { get => isAlertedAllEnemy; private set => isAlertedAllEnemy = value; }
}
