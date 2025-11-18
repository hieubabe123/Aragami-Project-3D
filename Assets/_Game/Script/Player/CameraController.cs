using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(CinemachineInputAxisController))]
public class CameraController : MonoBehaviour
{
    private CinemachineInputAxisController _axisController;

    // Biến để lưu ID của ngón tay đang điều khiển camera
    private int _lookTouchId = -1;

    void Awake()
    {
        _axisController = GetComponent<CinemachineInputAxisController>();
    }

    private void OnEnable()
    {
        // Kích hoạt hệ thống Enhanced Touch để có các sự kiện chạm đáng tin cậy
        EnhancedTouchSupport.Enable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += HandleFingerDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp += HandleFingerUp;

        // Tắt bộ điều khiển trục theo mặc định khi bắt đầu
        // để camera không xoay khi chưa có cú chạm hợp lệ.
        if (_axisController != null)
        {
            _axisController.enabled = false;
        }
    }

    private void OnDisable()
    {
        // Hủy đăng ký sự kiện để tránh lỗi rò rỉ bộ nhớ
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= HandleFingerDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp -= HandleFingerUp;

        // Tắt Enhanced Touch nếu không cần nữa
        if (EnhancedTouchSupport.enabled)
        {
            EnhancedTouchSupport.Disable();
        }

        // Bật lại bộ điều khiển để không ảnh hưởng đến các lần chạy sau hoặc trên PC
        if (_axisController != null)
        {
            _axisController.enabled = true;
        }
    }

    private void HandleFingerDown(Finger finger)
    {
        // Nếu đã có một ngón tay điều khiển camera, bỏ qua các ngón tay mới
        if (_lookTouchId != -1)
        {
            return;
        }

        // Kiểm tra xem ngón tay có bắt đầu ở nửa bên phải màn hình không
        if (finger.screenPosition.x > Screen.width / 2)
        {
            // Lưu ID của ngón tay này và BẬT bộ điều khiển trục
            _lookTouchId = finger.index;
            if (_axisController != null)
            {
                _axisController.enabled = true;
            }
        }
    }

    private void HandleFingerUp(Finger finger)
    {
        // Kiểm tra xem có phải ngón tay đang điều khiển camera vừa được nhấc lên không
        if (finger.index == _lookTouchId)
        {
            // Reset ID và TẮT bộ điều khiển trục
            _lookTouchId = -1;
            if (_axisController != null)
            {
                _axisController.enabled = false;
            }
        }
    }
}
