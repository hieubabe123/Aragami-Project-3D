using System.Collections;
using System.Collections.Generic;
using System.Linq; // Cần thêm để sử dụng LINQ
using UnityEngine;

public class PlayerChangeSkin : MonoBehaviour
{
    // === Hằng số và Biến có thể chỉnh sửa ===
    private const string SHADOW_TAG = "Shadow";
    [Tooltip("Thời gian (giây) để hoàn thành hiệu ứng chuyển màu.")]
    public float transitionDuration = 0.5f;

    // === Biến nội bộ ===
    private SkinnedMeshRenderer[] renderers;
    private Color _inShadowColor = Color.black;
    private Color _outShadowColor = Color.white;
    private MaterialPropertyBlock _materialPropertyBlock;
    private Coroutine _colorChangeCoroutine;

    // BIẾN MỚI: Danh sách để theo dõi tất cả các shadow đang va chạm
    private List<Collider> _activeShadows = new List<Collider>();

    void Awake()
    {
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        ApplyColor(_outShadowColor);
    }

    // HÀM MỚI: Kiểm tra và dọn dẹp danh sách shadow trong mỗi frame
    void Update()
    {
        // Nếu có bất kỳ shadow nào trong danh sách
        if (_activeShadows.Any())
        {
            // Đếm số lượng shadow đang active trước khi dọn dẹp
            int countBeforeCleaning = _activeShadows.Count;

            // Xóa các shadow đã bị null (destroy) hoặc bị disable khỏi danh sách
            _activeShadows.RemoveAll(s => s == null || !s.gameObject.activeInHierarchy);

            // Nếu trước đó có shadow và bây giờ danh sách trống (do shadow bị disable/destroy)
            // thì chuyển màu về trạng thái bình thường
            if (countBeforeCleaning > 0 && _activeShadows.Count == 0)
            {
                StartColorTransition(_outShadowColor);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(SHADOW_TAG))
        {
            // Chỉ thêm nếu shadow chưa có trong danh sách
            if (!_activeShadows.Contains(other))
            {
                _activeShadows.Add(other);
            }

            // Chỉ bắt đầu chuyển màu ĐEN nếu đây là vùng shadow ĐẦU TIÊN người chơi bước vào
            if (_activeShadows.Count == 1)
            {
                StartColorTransition(_inShadowColor);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(SHADOW_TAG))
        {
            _activeShadows.Remove(other);

            // Chỉ bắt đầu chuyển màu TRẮNG nếu người chơi đã ra khỏi TẤT CẢ các vùng shadow
            if (_activeShadows.Count == 0)
            {
                StartColorTransition(_outShadowColor);
            }
        }
    }

    private void StartColorTransition(Color targetColor)
    {
        if (_colorChangeCoroutine != null)
        {
            StopCoroutine(_colorChangeCoroutine);
        }
        _colorChangeCoroutine = StartCoroutine(SmoothlyChangeColor(targetColor));
    }

    private IEnumerator SmoothlyChangeColor(Color targetColor)
    {
        float elapsedTime = 0f;

        renderers[0].GetPropertyBlock(_materialPropertyBlock, 0);
        Color startColor = _materialPropertyBlock.GetColor("_BaseColor");

        while (elapsedTime < transitionDuration)
        {
            Color currentColor = Color.Lerp(startColor, targetColor, elapsedTime / transitionDuration);
            ApplyColor(currentColor);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ApplyColor(targetColor);
    }

    private void ApplyColor(Color color)
    {
        foreach (var renderer in renderers)
        {
            if (!renderer) continue;

            for (int i = 0; i < renderer.materials.Length; i++)
            {
                _materialPropertyBlock.SetColor("_BaseColor", color);
                renderer.SetPropertyBlock(_materialPropertyBlock, i);
            }
        }
    }
}