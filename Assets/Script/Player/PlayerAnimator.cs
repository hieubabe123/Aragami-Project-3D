using UnityEngine;



public class PlayerAnimator : MonoBehaviour
{
    private const string MOVE_SPEED_ANIM = "Speed";
    private const string IDLE_ID_ANIM = "IdleID";
    private const string STEALTH_KILL_ID = "StealthKillID";

    private const string ISMOVING = "isMoving";
    private const string ISIDLE = "isIdle";
    private const string ISDEAD = "isDead";
    private const string ISKILLING = "Stealth Kill";
    private const string WITHDRAWING = "Withdrawing";

    private float _currentIdleID;
    private float _currentMoveSpeed;

    private Animator _animator;
    private PlayerMovement _playerMovement;


    void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (_playerMovement.isDead)
        {
            SetTriggerDead();
            return;
        }
        CheckBoolIsIdle();
        CheckBoolIsMoving();
        CheckFloatMoveSpeed();
        CheckFloatIdleID();
    }

    private void CheckBoolIsIdle()
    {
        _animator.SetBool(ISIDLE, _playerMovement.isIdle);
    }

    private void CheckBoolIsMoving()
    {
        _animator.SetBool(ISMOVING, _playerMovement.isMoving);
    }

    private void CheckFloatMoveSpeed()
    {
        float _targetMoveSpeed = _playerMovement.currentSpeed;
        _currentMoveSpeed = Mathf.SmoothDamp(_currentMoveSpeed, _targetMoveSpeed, ref _currentMoveSpeed, .05f);
        _currentMoveSpeed = Mathf.Max(0, _currentMoveSpeed);
        _animator.SetFloat(MOVE_SPEED_ANIM, _currentMoveSpeed);
    }
    private void CheckFloatIdleID()
    {
        float _targetIdleID = _playerMovement.idleID;
        _currentIdleID = Mathf.SmoothDamp(_currentIdleID, _targetIdleID, ref _currentIdleID, .05f);
        _currentIdleID = Mathf.Clamp(_currentIdleID, 0, 1);
        _animator.SetFloat(IDLE_ID_ANIM, _currentIdleID);
    }

    private void SetTriggerDead()
    {
        _animator.SetTrigger(ISDEAD);


    }

    public void SetTriggerKillStealth(float stealthKillID)
    {
        _animator.SetTrigger(ISKILLING);
        _animator.SetFloat(STEALTH_KILL_ID, stealthKillID);

    }

    public void SetTriggerWithdraw()
    {
        _animator.SetTrigger(WITHDRAWING);
    }


}
