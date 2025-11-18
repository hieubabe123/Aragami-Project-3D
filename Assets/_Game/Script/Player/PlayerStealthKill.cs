using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStealthKill : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private PlayerAnimator _playerAnimator;
    private GameObject _currentTarget;

    [Header("----------------- Stealth Kill Data -------------------")]
    public List<StealthKillDataSO> stealthKillAnimationData;


    [Header("----------------- Snapping Settings -------------------")]
    [SerializeField] private float _snappingSpeed = 20f;
    [SerializeField] private float _rotationSpeed = 20f;
    [SerializeField] private float _withdrawingDuration = 1.5f;

    [Header("----------------- Gameobject Settings -------------------")]
    [SerializeField] private Transform katanaPos;
    [SerializeField] private Transform handHoldKatanaPos;
    [SerializeField] private Transform shealthPos;


    void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerAnimator = GetComponent<PlayerAnimator>();
    }
    public void SetKillTarget(GameObject target)
    {
        _currentTarget = target;
        EventDispatcher.Dispatch(new EventDefine.OnPlayerCanStealthKill { CanStealthKill = true });
    }

    public void ClearKillTarget(GameObject target)
    {
        if (_currentTarget != null)
        {
            _currentTarget = null;
            EventDispatcher.Dispatch(new EventDefine.OnPlayerCanStealthKill { CanStealthKill = false });
        }
    }

    public void OnStealthKillInput()
    {
        if (_currentTarget != null)
        {
            StartCoroutine(ExecuteStealthKill());
        }
    }

    private IEnumerator ExecuteStealthKill()
    {
        EnemyState enemyState = _currentTarget.GetComponent<EnemyState>();
        EnemyMovement enemyMovement = _currentTarget.GetComponent<EnemyMovement>();
        EnemyAnimator enemyAnimator = _currentTarget.GetComponent<EnemyAnimator>();
        Rigidbody rb = GetComponent<Rigidbody>();

        if (enemyState == null || enemyAnimator == null)
        {
            yield break;
        }

        _playerMovement.SetMovementLocked(true);
        enemyState.SetMovementLocked(true);
        enemyMovement.StopMovement();

        _playerAnimator.SetTriggerWithdraw();


        int stealthKillAndDeadID = Random.Range(0, stealthKillAnimationData.Count);
        StealthKillDataSO selectedKill = stealthKillAnimationData[stealthKillAndDeadID];

        Vector3 targetPosition = _currentTarget.transform.position - _currentTarget.transform.forward * selectedKill.PositionDistance;
        Quaternion targetRotation = _currentTarget.transform.rotation;

        while (Vector3.Distance(transform.position, targetPosition) > 0.05f || Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _snappingSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            yield return null;
        }

        rb.MovePosition(targetPosition);
        rb.MoveRotation(targetRotation);

        enemyMovement.SetNavMeshDisable();
        EventDispatcher.Dispatch(new EventDefine.OnPlayerKilledEnemy { BotTrigger = _currentTarget.GetComponent<StealthKillTrigger>() });


        yield return new WaitForSeconds(_withdrawingDuration);


        _playerAnimator.SetTriggerKillStealth(stealthKillAndDeadID);
        enemyAnimator.ExecuteDeadAnimation(stealthKillAndDeadID);

        yield return new WaitForSeconds(selectedKill.AnimationDuration);

        enemyState.SetState(EnemyStateTypes.Dead);

        _playerMovement.SetMovementLocked(false);
        _currentTarget = null;

    }
    public void EquipKatanaWeapon()
    {
        katanaPos.SetParent(handHoldKatanaPos);
        katanaPos.localPosition = Vector3.zero;
        katanaPos.localRotation = Quaternion.identity;
    }

    public void UnequipKatanaWeapon()
    {
        katanaPos.SetParent(shealthPos);
        katanaPos.localPosition = Vector3.zero;
        katanaPos.localRotation = Quaternion.identity;
    }

}
