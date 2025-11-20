using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEffect : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{

    Transform _transform;
    private void Start()
    {
        _transform = GetComponent<Transform>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_transform)
        {
            _transform.transform.DOScale(0.9f, 0.2f).SetEase(Ease.Linear).SetUpdate(true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_transform)
        {
            _transform.transform.DOScale(1f, 0.2f).SetEase(Ease.Linear).SetUpdate(true);
        }
    }
}
