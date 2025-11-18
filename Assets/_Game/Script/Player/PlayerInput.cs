using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    private InputSystem_Actions _inputActions;
    private PlayerMovement _playerMovement;
    private PlayerCrosshairController _playerCrosshairController;
    private PlayerStealthKill _playerStealthKill;
    private PlayerCreateShadow _playerCreateShadow;
    [Header("------------------------Joystick Settings------------------------")]
    [SerializeField] private FloatingJoystick floatingJoystick;

    [Header("------------------------Buttons Held States------------------------")]
    public bool runButtonHeld;
    public bool crouchButtonHeld;

    [Header("------------------------Movement Input------------------------")]
    public Vector2 moveInput;

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerCrosshairController = GetComponent<PlayerCrosshairController>();
        _playerStealthKill = GetComponent<PlayerStealthKill>();
        _playerCreateShadow = GetComponent<PlayerCreateShadow>();

        //Crouch and Run button actions
        _inputActions.Player.Crouch.started += context => crouchButtonHeld = true;
        _inputActions.Player.Crouch.canceled += context => crouchButtonHeld = false;

        _inputActions.Player.Sprint.started += context => runButtonHeld = true;
        _inputActions.Player.Sprint.canceled += context => runButtonHeld = false;

        _inputActions.Player.Teleport.started += StartAiming;
        _inputActions.Player.Teleport.canceled += AttemptTeleportOnRelease;

        _inputActions.Player.Kill.performed += Kill;
        _inputActions.Player.CreateShadow.performed += CreateShadow;
    }

    private void Start()
    {
        runButtonHeld = false;
        crouchButtonHeld = false;
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
    }

    private void Update()
    {
        if (_playerMovement.isDead)
        {
            return;
        }
        GetMovementVectorNormalizedInput();
        HandleAllMovementState();
    }

    public Vector2 GetMovementVectorNormalizedInput()
    {
        if (floatingJoystick != null)
        {
            moveInput = new Vector2(floatingJoystick.Horizontal, floatingJoystick.Vertical).normalized;
        }
        else
        {
            moveInput = _inputActions.Player.Move.ReadValue<Vector2>().normalized;
        }
        return moveInput;
    }

    private void HandleAllMovementState()
    {
        if (HandleCrouchState())
        {
            return;
        }
        if (HandleRunState())
        {
            return;
        }
        HandleWalkAndIdleState();
    }
    private bool HandleCrouchState()
    {
        if (crouchButtonHeld)
        {
            if (moveInput.magnitude > 0.1f)
            {
                SetPlayerMovementState(MovementState.Crouching);
            }
            else
            {
                SetPlayerMovementState(MovementState.IdleCrouching);
            }
            return true;
        }
        return false;
    }

    private bool HandleRunState()
    {
        if (runButtonHeld)
        {
            if (moveInput.magnitude > 0.1f)
            {
                SetPlayerMovementState(MovementState.Running);
            }
            else
            {
                SetPlayerMovementState(MovementState.Idle);
            }
            return true;
        }
        return false;
    }

    private void HandleWalkAndIdleState()
    {
        if (moveInput.magnitude > 0.1f)
        {
            SetPlayerMovementState(MovementState.Walking);
        }
        else
        {
            SetPlayerMovementState(MovementState.Idle);
        }
    }


    private void StartAiming(InputAction.CallbackContext context)
    {
        if (_playerMovement.isDead)
        {
            return;
        }
        if (_playerCrosshairController != null)
        {
            _playerCrosshairController.SetAiming(true);
        }
    }

    private void AttemptTeleportOnRelease(InputAction.CallbackContext context)
    {

        if (_playerMovement.isDead)
        {
            return;
        }

        if (_playerCrosshairController != null && _playerCrosshairController.CanTeleport)
        {

            Vector3 targetPosition = _playerCrosshairController.TeleportPosition;
            _playerMovement.ExecuteTeleport(targetPosition);
        }

        // Dù có dịch chuyển được hay không, sau khi thả nút là phải tắt chế độ ngắm
        if (_playerCrosshairController != null)
        {
            _playerCrosshairController.SetAiming(false);
        }
    }

    private void SetPlayerMovementState(MovementState state)
    {
        _playerMovement.currentState = state;
    }

    private void Kill(InputAction.CallbackContext context)
    {
        _playerStealthKill.OnStealthKillInput();
    }

    private void CreateShadow(InputAction.CallbackContext context)
    {
        if (_playerMovement.isDead)
        {
            return;
        }
        if (_playerCreateShadow != null)
        {
            Vector3 targetPosition = new Vector3(_playerCrosshairController.ShadowCreatePosition.x, 0.3f, _playerCrosshairController.ShadowCreatePosition.z);
            _playerCreateShadow.CreateShadowAt(targetPosition);
        }

    }

    #region Public Methods to Access Input Actions
    public void OnTeleportButtonDown()
    {
        if (_playerMovement.isDead)
        {
            return;
        }
        if (_playerCrosshairController != null)
        {
            // Logic từ hàm StartAiming cũ
            _playerCrosshairController.SetAiming(true);
        }
    }

    public void OnTeleportButtonUp()
    {
        if (_playerMovement.isDead)
        {
            return;
        }

        if (_playerCrosshairController != null && _playerCrosshairController.CanTeleport)
        {
            Vector3 targetPosition = _playerCrosshairController.TeleportPosition;
            _playerMovement.ExecuteTeleport(targetPosition);
        }
        if (_playerCrosshairController != null)
        {
            _playerCrosshairController.SetAiming(false);
        }
    }

    public void OnCreateShadowButtonPressed()
    {
        if (_playerMovement.isDead) return;

        if (_playerCreateShadow != null)
        {
            // Logic này được chuyển từ hàm CreateShadow(context) cũ
            Vector3 targetPosition = new Vector3(_playerCrosshairController.ShadowCreatePosition.x, 0.3f, _playerCrosshairController.ShadowCreatePosition.z);
            _playerCreateShadow.CreateShadowAt(targetPosition);
        }
    }

    public void OnRunButtonDown()
    {
        runButtonHeld = true;
    }

    public void OnRunButtonUp()
    {
        runButtonHeld = false;
    }

    public void OnCrouchButtonDown()
    {
        crouchButtonHeld = true;
    }

    public void OnCrouchButtonUp()
    {
        crouchButtonHeld = false;
    }

    #endregion

}
