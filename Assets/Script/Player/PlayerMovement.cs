using System.Collections;
using UnityEngine;



public enum MovementState
{
    Idle,
    Walking,
    Running,
    Crouching,
    IdleCrouching,
}

public class PlayerMovement : MonoBehaviour
{
    [Header("------------------------Player Settings------------------------")]
    private PlayerInput _playerInput;
    private Rigidbody _rb;
    private DissolvingControllerTut _dissolvingController;
    private MeshTrailTut _meshTrailTut;

    [Header("------------------------Camera Settings------------------------")]
    [SerializeField] private Transform cameraTransform;


    [Header("------------------------Movement States------------------------")]
    public MovementState currentState;
    public bool isMoving;
    public bool isIdle;
    public int idleID;
    public bool isDead;
    public bool isKilling;
    private bool _isTeleporting;

    [Header("------------------------Teleport Settings------------------------")]
    [SerializeField] private float teleportTravelDuration = 0.4f;


    [Header("------------------------Movement References------------------------")]
    private Vector3 _moveDirection;
    private Vector2 _moveInput;
    private Vector3 _camForward;
    private Vector3 _camRight;
    private bool _isLocked;
    public float currentSpeed;
    public float walkSpeed = 5f;
    public float runSpeed = 7f;
    public float crouchSpeed = 3f;
    private float _rotationSpeed = 20f;
    private float _dissolveTeleportDuration = 0.5f;
    private float _appearTeleportDuration = 0.6f;
    private float _dissolveDeadDuration = 3.5f;
    private float _appearDeadDuration = 2f;

    [Header("------------------------GameObject Settings------------------------")]
    [SerializeField] private Cloth scrafCloth;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rb = GetComponentInChildren<Rigidbody>();
        _dissolvingController = GetComponent<DissolvingControllerTut>();
        _meshTrailTut = GetComponent<MeshTrailTut>();
    }

    void Start()
    {
        StartCoroutine(_dissolvingController.AppearCo(_appearDeadDuration));
        _isTeleporting = false;
        isDead = false;
        currentState = MovementState.Idle;
        idleID = 0;
    }

    void Update()
    {
        if (_isTeleporting || isDead || _isLocked)
        {
            return;
        }

        _moveInput = _playerInput.GetMovementVectorNormalizedInput();
        RotateOrientation();
    }

    void FixedUpdate()
    {
        if (_isTeleporting || isDead || _isLocked)
        {
            return;
        }
        currentSpeed = GetSpeedFromState();
        Move(currentSpeed);

    }

    private float GetSpeedFromState()
    {
        switch (currentState)
        {
            case MovementState.Walking:
                isMoving = true;
                isIdle = false;
                return walkSpeed;
            case MovementState.Running:
                isMoving = true;
                isIdle = false;
                return runSpeed;
            case MovementState.Crouching:
                isMoving = true;
                isIdle = false;
                return crouchSpeed;
            case MovementState.IdleCrouching:
                isMoving = false;
                isIdle = true;
                idleID = 1;
                return 0f;
            default:
                isMoving = false;
                isIdle = true;
                idleID = 0;
                return 0f;
        }
    }

    private void Move(float moveSpeed)
    {
        if (_moveInput.sqrMagnitude < 0.01f)
        {
            _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
            return;
        }

        CalculateMoveDirection();
        _moveDirection.Normalize();

        Vector3 targetVelocity = _moveDirection * moveSpeed;
        _rb.linearVelocity = new Vector3(targetVelocity.x, _rb.linearVelocity.y, targetVelocity.z);

    }

    private void RotateOrientation()
    {
        CalculateMoveDirection();
        if (_moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    private void CalculateMoveDirection()
    {
        _camForward = cameraTransform.forward;
        _camRight = cameraTransform.right;

        _camForward.y = 0f;
        _camRight.y = 0f;

        _camForward.Normalize();
        _camRight.Normalize();

        _moveDirection = _camForward * _moveInput.y + _camRight * _moveInput.x;
    }

    public void ExecuteTeleport(Vector3 targetPosition)
    {
        StartCoroutine(TeleportSequence(targetPosition));

    }

    private IEnumerator TeleportSequence(Vector3 targetPosition)
    {
        _isTeleporting = true;
        _rb.linearVelocity = Vector3.zero;
        Vector3 startPosition = _rb.position;
        // --- GIAI ĐOẠN 1: BIẾN MẤT ---
        yield return StartCoroutine(_dissolvingController.DissolveCo(_dissolveTeleportDuration));
        if (scrafCloth != null)
        {
            scrafCloth.enabled = false;
        }

        // --- GIAI ĐOẠN 2: DỊCH CHUYỂN ---
        _dissolvingController.skinnedMesh.enabled = false;

        if (_meshTrailTut != null)
        {
            yield return StartCoroutine(_meshTrailTut.PlayerTrail(startPosition, targetPosition, teleportTravelDuration));
        }

        _rb.MovePosition(targetPosition);


        // --- GIAI ĐOẠN 3: XUẤT HIỆN LẠI ---
        _dissolvingController.skinnedMesh.enabled = true;

        yield return StartCoroutine(_dissolvingController.AppearCo(_appearTeleportDuration));
        if (scrafCloth != null)
        {
            scrafCloth.enabled = true;
        }
        _isTeleporting = false;
    }

    public void Dead()
    {
        if (isDead) return;

        isDead = true;
        DissolvingControllerTut dissolveVfx = GetComponent<DissolvingControllerTut>();
        StartCoroutine(dissolveVfx.DissolveCo(_dissolveDeadDuration));
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }
    public void SetMovementLocked(bool isLocked)
    {
        this._isLocked = isLocked;
    }
}
