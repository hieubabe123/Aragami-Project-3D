using UnityEngine;


[CreateAssetMenu(fileName = "StealthKill", menuName = "Data/Stealth Kill Animation")]
public class StealthKillDataSO : ScriptableObject
{
    [SerializeField] private string _animationName;
    public string AnimationName { get => _animationName; private set => _animationName = value; }

    [SerializeField] private float _animationDuration;
    public float AnimationDuration { get => _animationDuration; private set => _animationDuration = value; }

    [SerializeField] private float _positionDistance;
    public float PositionDistance { get => _positionDistance; private set => _positionDistance = value; }
}
