using System.Collections;
using UnityEngine;

public enum EnemyStateTypes
{
    Patrolling,
    Suspecting,
    AlertingAllEnemy,
    Chasing,
    Attacking,
    Dead
}

public class EnemyState : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private EnemyStateTypes currentState;

    [Header("Components")]
    private EnemyMovement _enemyMovement;
    private EnemyFieldOfView _fov;
    private EnemyAnimator _animator;
    private Transform _playerDetectionTarget;
    private CapsuleCollider _capsuleCollider;
    private EnemyIcons _icons;
    [SerializeField] private EnemyScriptableObject EnemyDataSO;

    [Header("-------------- GameObject Children")]
    [SerializeField] private Transform hornSpeakerPos;
    [SerializeField] private Transform bodyHoldHornSpeakerPos;
    [SerializeField] private Transform handHoldHornSpeakerPos;

    [Header("Timers & Durations & Distances")]

    // Biến nội bộ
    private float _currentStateTimer;
    private float _lastAttackTime = -999f;
    private Vector3 _lastKnownPlayerPosition;
    private bool isDead;
    private bool _isLocked;
    private bool _isHostile;
    private bool _isAlertingInProgress = false;
    private Coroutine _alertCoroutine;
    public bool isAlertedByAllEnemy = false;

    private void Awake()
    {
        _enemyMovement = GetComponent<EnemyMovement>();
        _fov = GetComponentInChildren<EnemyFieldOfView>();
        _animator = GetComponent<EnemyAnimator>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _icons = GetComponent<EnemyIcons>();
    }

    private void OnEnable()
    {
        EventDispatcher.AddListener<EventDefine.OnAlertAllEnemy>(HandleAlertAllEnemy);
    }

    private void OnDisable()
    {
        EventDispatcher.RemoveListener<EventDefine.OnAlertAllEnemy>(HandleAlertAllEnemy);
    }

    private void HandleAlertAllEnemy(EventDefine.OnAlertAllEnemy onEnemyAlert)
    {
        if (isAlertedByAllEnemy) return;
        isAlertedByAllEnemy = true;

        // Nếu đang trong trạng thái bình thường, chuyển sang hostile
        if (currentState == EnemyStateTypes.Patrolling || currentState == EnemyStateTypes.Suspecting)
        {
            _isHostile = true;
        }
    }

    void Start()
    {
        isDead = false;
        _isHostile = false;

        if (_enemyMovement.patrolPoints.Length > 0)
        {
            SetState(EnemyStateTypes.Patrolling);
        }
        else
        {
            SetState(EnemyStateTypes.Patrolling);
        }
    }

    void Update()
    {
        if (isDead || _isLocked)
        {
            return;
        }

        // Cập nhật player detection
        if (_fov.HasTargetInDetection())
        {
            _playerDetectionTarget = _fov.GetClosestDetectionTarget();
            _lastKnownPlayerPosition = _playerDetectionTarget.position;
        }
        else
        {
            _playerDetectionTarget = null;
        }

        // Xử lý state machine
        switch (currentState)
        {
            case EnemyStateTypes.Patrolling:
                HandlePatrolState();
                break;
            case EnemyStateTypes.Suspecting:
                HandleSuspectingState();
                break;
            case EnemyStateTypes.AlertingAllEnemy:
                HandleAlertingAllEnemyState();
                break;
            case EnemyStateTypes.Chasing:
                HandleChaseState();
                break;
            case EnemyStateTypes.Attacking:
                HandleAttackState();
                break;
            case EnemyStateTypes.Dead:
                break;
        }

        HandleRotation();
    }

    private void HandleRotation()
    {
        // Chỉ xoay khi không đang trong trạng thái patrol hoặc dead
        if (currentState == EnemyStateTypes.Patrolling || currentState == EnemyStateTypes.Dead)
            return;

        Vector3 lookPosition;

        // Quyết định xem nên nhìn vào đâu dựa trên state hiện tại
        if (_playerDetectionTarget != null)
            lookPosition = _playerDetectionTarget.position;
        else
            lookPosition = _lastKnownPlayerPosition;

        // Thực hiện xoay mượt mà (chỉ khi không đang chase - vì chase đã có rotation riêng)
        if (currentState != EnemyStateTypes.Chasing)
        {
            Vector3 direction = (lookPosition - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, EnemyDataSO.RotationSpeed * Time.deltaTime);
            }
        }
    }

    private void HandlePatrolState()
    {
        // Nếu được alert bởi enemy khác và thấy player
        if (isAlertedByAllEnemy && _playerDetectionTarget != null)
        {
            _isHostile = true;
            SetState(EnemyStateTypes.Chasing);
            return;
        }

        // Nếu thấy player lần đầu
        if (_playerDetectionTarget != null)
        {
            SetState(EnemyStateTypes.Suspecting);
            return;
        }
    }

    private void HandleSuspectingState()
    {
        _currentStateTimer += Time.deltaTime;

        if (_playerDetectionTarget != null) // Nếu vẫn đang thấy người chơi
        {
            if (_currentStateTimer >= EnemyDataSO.SuspectDuration)
            {
                _isHostile = true;

                // Nếu chưa alert và chưa bị alert bởi người khác
                if (!isAlertedByAllEnemy && !_isAlertingInProgress)
                {
                    SetState(EnemyStateTypes.AlertingAllEnemy);
                }
                else
                {
                    // Đã được alert hoặc đang alert, chuyển thẳng sang chasing/attacking
                    if (_fov.HasTargetInAttackRange())
                    {
                        SetState(EnemyStateTypes.Attacking);
                    }
                    else
                    {
                        SetState(EnemyStateTypes.Chasing);
                    }
                }
            }
        }
        else // Nếu đã mất dấu người chơi
        {
            if (_currentStateTimer >= EnemyDataSO.SearchDuration)
            {
                _isHostile = false;
                SetState(EnemyStateTypes.Patrolling);
            }
        }
    }

    private void HandleAlertingAllEnemyState()
    {
        // State này sẽ được kết thúc bởi animation event hoặc coroutine
        // Không cần xử lý gì thêm ở đây
    }

    private void HandleChaseState()
    {
        if (_playerDetectionTarget == null)
        {
            SetState(EnemyStateTypes.Suspecting);
            return;
        }

        _isHostile = true;
        _enemyMovement.ChaseTarget(_playerDetectionTarget);

        if (_fov.HasTargetInAttackRange())
        {
            SetState(EnemyStateTypes.Attacking);
        }
    }

    private void HandleAttackState()
    {
        // ĐIỀU KIỆN THOÁT MỚI: Chỉ quay lại Chasing nếu Player ở xa hơn (attackRadius * attackLeash)
        if (_playerDetectionTarget == null || Vector3.Distance(transform.position, _playerDetectionTarget.position) > _fov.attackRadius * EnemyDataSO.AttackLeash)
        {
            SetState(EnemyStateTypes.Chasing);
            return;
        }

        // Luôn nhìn về phía Player khi tấn công
        if (_playerDetectionTarget != null)
        {
            Vector3 direction = (_playerDetectionTarget.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        if (Time.time >= _lastAttackTime + EnemyDataSO.AttackCooldown)
        {
            _lastAttackTime = Time.time;
            _animator.ExecuteAttackAnimation();
        }
    }

    private IEnumerator AlertAllEnemyCoroutine()
    {
        _isAlertingInProgress = true;

        // Chờ animation hoàn thành (có thể điều chỉnh thời gian này)
        yield return new WaitForSeconds(2f);

        _isAlertingInProgress = false;
        _alertCoroutine = null;

        // Sau khi hoàn thành alert, quyết định state tiếp theo
        if (_playerDetectionTarget != null)
        {
            if (_fov.HasTargetInAttackRange())
            {
                SetState(EnemyStateTypes.Attacking);
            }
            else
            {
                SetState(EnemyStateTypes.Chasing);
            }
        }
        else
        {
            SetState(EnemyStateTypes.Suspecting);
        }
    }

    public void SetState(EnemyStateTypes newState)
    {
        if (currentState == newState) return;

        // Cleanup state cũ
        if (_alertCoroutine != null)
        {
            StopCoroutine(_alertCoroutine);
            _alertCoroutine = null;
            _isAlertingInProgress = false;
        }

        EnemyStateTypes previousState = currentState;
        currentState = newState;

        // Reset timer khi chuyển state
        _currentStateTimer = 0f;

        // Bắt đầu hành động của state mới
        switch (currentState)
        {
            case EnemyStateTypes.Patrolling:
                _enemyMovement.StartPatrolling();
                _icons.ShowQuestionMarkIcon();
                break;

            case EnemyStateTypes.Suspecting:
                _enemyMovement.StopMovement();
                _icons.ShowQuestionMarkIcon();
                break;

            case EnemyStateTypes.AlertingAllEnemy:
                _enemyMovement.StopMovement();
                _icons.ShowAlertAllEnemyIcon();

                // Chỉ dispatch event nếu chưa được alert
                if (!isAlertedByAllEnemy)
                {
                    EventDispatcher.Dispatch<EventDefine.OnAlertAllEnemy>();
                    isAlertedByAllEnemy = true;
                }

                _animator.ExecuteAlertAllEnemyAnimation();
                _alertCoroutine = StartCoroutine(AlertAllEnemyCoroutine());
                break;

            case EnemyStateTypes.Attacking:
                _enemyMovement.StopMovement();
                _icons.ShowAttackMarkIcon();
                break;

            case EnemyStateTypes.Chasing:
                _icons.ShowExclamationMarkIcon();
                // Không gọi movement ở đây vì sẽ được gọi trong HandleChaseState
                break;

            case EnemyStateTypes.Dead:
                Die();
                _enemyMovement.SetNavMeshDisable();
                break;
        }

        Debug.Log($"Enemy {gameObject.name}: {previousState} -> {currentState}");
    }

    public void SetMovementLocked(bool isLocked)
    {
        this._isLocked = isLocked;
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;
        _enemyMovement.StopMovement();
        _capsuleCollider.isTrigger = true;
    }

    public void SetBodyHoldHornSpeaker()
    {
        hornSpeakerPos.SetParent(bodyHoldHornSpeakerPos);
        hornSpeakerPos.localPosition = Vector3.zero;
        hornSpeakerPos.localRotation = Quaternion.identity;
    }

    public void SetHandHoldHornSpeaker()
    {
        hornSpeakerPos.SetParent(handHoldHornSpeakerPos);
        hornSpeakerPos.localPosition = Vector3.zero;
        hornSpeakerPos.localRotation = Quaternion.identity;
    }

    // Getter cho EnemyAnimator để check trạng thái
    public bool IsMoving()
    {
        return (currentState == EnemyStateTypes.Patrolling || currentState == EnemyStateTypes.Chasing) && !_isLocked && !isDead;
    }

    public EnemyStateTypes GetCurrentState()
    {
        return currentState;
    }
}