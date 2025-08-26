using System.Collections;
using UnityEngine;

public enum EnemyStateTypes
{
    Patrolling,
    Alerting,
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

    [Header("-------------- GameObject Children")]
    [SerializeField] private Transform hornSpeakerPos;
    [SerializeField] private Transform bodyHoldHornSpeakerPos;
    [SerializeField] private Transform handHoldHornSpeakerPos;


    [Header("Timers & Durations & Distances")]
    [SerializeField] private float alertDuration = 3f;
    [SerializeField] private float searchDuration = 7f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackLeash = 1.2f;

    // Biến nội bộ
    private float _currentStateTimer;
    private float _lastAttackTime = -999f;
    private float rotationSpeed = 5f;
    private float _timeToAlertAllEnemies = 5f;
    private Vector3 _lastKnownPlayerPosition;
    private bool isDead;
    private bool _isLocked;
    private bool _isHostile;
    private bool _isAlertedByAllEnemy;

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

    private void HandleAlertAllEnemy(EventDefine.OnAlertAllEnemy onAlert)
    {

    }

    void Start()
    {
        isDead = false;
        _isHostile = false;
        if (_enemyMovement.patrolPoints.Length > 0)
        {
            currentState = EnemyStateTypes.Patrolling;
            _enemyMovement.StartPatrolling();
        }
        else
        {
            currentState = EnemyStateTypes.Patrolling;
            _enemyMovement.StopMovement();
        }
    }

    void Update()
    {
        if (isDead || _isLocked)
        {
            return;
        }
        if (_fov.visibleDetectionTargets.Count > 0)
        {
            _playerDetectionTarget = _fov.visibleDetectionTargets[0];
            _lastKnownPlayerPosition = _playerDetectionTarget.position;
        }
        else
        {
            _playerDetectionTarget = null;
        }

        switch (currentState)
        {
            case EnemyStateTypes.Patrolling:
                HandlePatrolState();
                break;
            case EnemyStateTypes.Alerting:
                HandleAlertedState();
                break;
            case EnemyStateTypes.Chasing:
                HandleChaseState();
                break;
            case EnemyStateTypes.Attacking:
                HandleAttackState();
                break;
            case EnemyStateTypes.AlertingAllEnemy:
                HandleAlertedState();
                break;
            case EnemyStateTypes.Dead:
                break;
        }
        HandleRotation();
    }

    private void HandleRotation()
    {
        Vector3 lookPosition;

        // Quyết định xem nên nhìn vào đâu dựa trên state hiện tại
        if (currentState == EnemyStateTypes.Alerting || currentState == EnemyStateTypes.Chasing || currentState == EnemyStateTypes.Attacking)
        {
            if (_playerDetectionTarget != null)
                lookPosition = _playerDetectionTarget.position;
            else
                lookPosition = _lastKnownPlayerPosition;
        }
        else
        {
            return;
        }

        // Thực hiện xoay mượt mà
        Vector3 direction = (lookPosition - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // HÀM NÀY BÂY GIỜ CHỈ KIỂM TRA ĐIỀU KIỆN
    private void HandlePatrolState()
    {
        if (_isHostile && _fov.visibleDetectionTargets.Count > 0)
        {
            if (!_isAlertedByAllEnemy)
            {
                SetState(EnemyStateTypes.AlertingAllEnemy);
            }
            else
            {
                SetState(EnemyStateTypes.Chasing);
            }
        }
        else if (_playerDetectionTarget != null)
        {
            SetState(EnemyStateTypes.Alerting);
        }
    }

    private void HandleAlertedState()
    {
        _enemyMovement.StopMovement();
        if (_playerDetectionTarget != null) // Nếu vẫn đang thấy người chơi
        {
            _currentStateTimer += Time.deltaTime;


            if (_currentStateTimer >= alertDuration)
            {
                _isHostile = true;
                if (_fov.visibleAttackTargets.Count > 0)
                {
                    SetState(EnemyStateTypes.Attacking);
                }
                else if (_fov.visibleDetectionTargets.Count > 0 && _fov.visibleAttackTargets.Count == 0)
                {
                    SetState(EnemyStateTypes.Chasing);
                }
            }
        }
        else // Nếu đã mất dấu người chơi
        {
            _currentStateTimer += Time.deltaTime;
            if (_currentStateTimer >= searchDuration)
            {
                _isHostile = false;
                SetState(EnemyStateTypes.Patrolling);
            }
        }
    }

    private void HandleChaseState()
    {
        if (_playerDetectionTarget == null)
        {
            SetState(EnemyStateTypes.Alerting);
            return;
        }
        _isHostile = true;
        _enemyMovement.ChaseTarget(_playerDetectionTarget);
        if (_fov.visibleAttackTargets.Count > 0)
        {
            SetState(EnemyStateTypes.Attacking);
        }
    }

    private void HandleAttackState()
    {
        _enemyMovement.StopMovement();

        // ĐIỀU KIỆN THOÁT MỚI: Chỉ quay lại Chasing nếu Player ở xa hơn (attackRadius * attackLeash)
        if (_playerDetectionTarget == null || Vector3.Distance(transform.position, _playerDetectionTarget.position) > _fov.attackRadius * attackLeash)
        {
            SetState(EnemyStateTypes.Chasing);
            return;
        }

        // Luôn nhìn về phía Player khi tấn công
        transform.LookAt(_playerDetectionTarget);

        if (Time.time >= _lastAttackTime + attackCooldown)
        {
            _lastAttackTime = Time.time;
            _animator.ExecuteAttackAnimation();
        }
    }

    private void HandleAlertAllEnemy()
    {
        _enemyMovement.StopMovement();
        _timeToAlertAllEnemies -= Time.deltaTime;
        if (_timeToAlertAllEnemies <= 0f)
        {
            _animator.ExecuteAlertAllEnemyAnimation();
        }
        EventDispatcher.Dispatch<EventDefine.OnAlertAllEnemy>();
    }

    // HÀM NÀY SẼ RA LỆNH CHO MOVEMENT
    public void SetState(EnemyStateTypes newState)
    {
        if (currentState == newState) return;

        // Dừng hành động của state cũ trước khi chuyển
        if (currentState == EnemyStateTypes.Patrolling)
        {
            _enemyMovement.StopMovement();
        }

        currentState = newState;

        // Bắt đầu hành động của state mới
        switch (currentState)
        {
            case EnemyStateTypes.Patrolling:
                _enemyMovement.StartPatrolling();
                _icons.ShowQuestionMarkIcon();
                break;
            case EnemyStateTypes.Alerting:
                _enemyMovement.StopMovement();
                _icons.ShowExclamationMarkIcon();
                _currentStateTimer = 0f;
                break;
            case EnemyStateTypes.AlertingAllEnemy:
                _enemyMovement.StopMovement();
                _icons.ShowExclamationMarkIcon();
                _currentStateTimer = 0f;
                _isAlertedByAllEnemy = true;
                HandleAlertAllEnemy();
                break;
            case EnemyStateTypes.Attacking:
                _icons.ShowAttackMarkIcon();
                break;
            case EnemyStateTypes.Chasing:
                _icons.ShowQuestionMarkIcon();
                break;
            case EnemyStateTypes.Dead:
                Die();
                _enemyMovement.SetNavMeshDisable();
                break;
        }
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
}