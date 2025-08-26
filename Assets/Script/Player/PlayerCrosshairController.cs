using UnityEngine;

public class PlayerCrosshairController : MonoBehaviour
{
    [Header("---------------------Crosshair Reference---------------------")]
    [SerializeField] private Transform _crosshairUI;
    [SerializeField] private Transform _crosshairCannotTeleport;
    [SerializeField] private Transform _crosshairTeleport;
    [SerializeField] private Transform _crosshairDefault;
    [SerializeField] private Camera _aimCamera;
    [SerializeField] private float _maxDistance = 10f;
    [SerializeField] private float _crosshairOffsetMultiplier = 0.01f;
    [SerializeField] private LayerMask _raycastMask = ~0;

    [Header("---------------------Teleport Reference---------------------")]
    public bool isAimingTeleport;
    public bool CanTeleport { get; private set; }
    public Vector3 TeleportPosition { get; private set; }
    public Vector3 ShadowCreatePosition { get; private set; }

    void Start()
    {
        SetCrosshairDefault();
        isAimingTeleport = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        CheckTeleportPosibility();
    }

    private void CheckTeleportPosibility()
    {

        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = _aimCamera.ScreenPointToRay(screenCenter);

        Vector3 targetPos;
        if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _raycastMask))
        {
            if (isAimingTeleport)
            {
                if (!IsInPointLight(hit.point))
                {
                    CanTeleport = true;
                    SetCrosshairTeleport();
                    TeleportPosition = hit.point + hit.normal * _crosshairOffsetMultiplier;
                }
                else
                {
                    CanTeleport = false;
                    SetCrosshairCannotTeleport();
                }
            }
            else
            {
                CanTeleport = false;
                SetCrosshairDefault();
                ShadowCreatePosition = hit.point + hit.normal * _crosshairOffsetMultiplier;
            }

            targetPos = hit.point + hit.normal * _crosshairOffsetMultiplier;
            _crosshairUI.position = targetPos;
            _crosshairUI.rotation = Quaternion.LookRotation(hit.normal);
        }
        else
        {
            CanTeleport = false;
            if (isAimingTeleport)
            {
                SetCrosshairCannotTeleport();
            }
            else
            {
                SetCrosshairDefault();
            }
            targetPos = ray.GetPoint(_maxDistance);
            _crosshairUI.position = targetPos;
            _crosshairUI.forward = _aimCamera.transform.forward;
        }


    }

    private bool IsInPointLight(Vector3 point)
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(point, _maxDistance);
        foreach (var col in nearbyColliders)
        {
            if (col.TryGetComponent<Light>(out Light light))
            {
                if (!light.enabled)
                {
                    continue;
                }


                float distanceToLight = Vector3.Distance(point, light.transform.position);
                if (distanceToLight > light.range)
                {
                    continue;
                }

                Vector3 directionToPoint = (point - light.transform.position).normalized;
                float distanceToPoint = Vector3.Distance(light.transform.position, point);

                if (Physics.Raycast(light.transform.position, directionToPoint, distanceToPoint - 0.02f, _raycastMask, QueryTriggerInteraction.Ignore))
                {
                    continue;
                }

                return true;
            }
        }
        return false;
    }

    private void SetCrosshairCannotTeleport()
    {
        _crosshairCannotTeleport.gameObject.SetActive(true);
        _crosshairTeleport.gameObject.SetActive(false);
        _crosshairDefault.gameObject.SetActive(false);
    }

    private void SetCrosshairTeleport()
    {
        _crosshairCannotTeleport.gameObject.SetActive(false);
        _crosshairTeleport.gameObject.SetActive(true);
        _crosshairDefault.gameObject.SetActive(false);
    }

    private void SetCrosshairDefault()
    {
        _crosshairCannotTeleport.gameObject.SetActive(false);
        _crosshairTeleport.gameObject.SetActive(false);
        _crosshairDefault.gameObject.SetActive(true);
    }

    public void SetAiming(bool aimingStatus)
    {
        isAimingTeleport = aimingStatus;
        _crosshairDefault.gameObject.SetActive(!aimingStatus);

        // Nếu tắt ngắm, reset lại trạng thái
        if (isAimingTeleport)
        {
            _crosshairDefault.gameObject.SetActive(false);
        }
        else
        {
            SetCrosshairDefault();
            CanTeleport = false;
        }
    }
}
