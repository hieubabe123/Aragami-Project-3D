using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

public class HandleButtonController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private OnScreenButton button;

    void Awake()
    {
        button = GetComponent<OnScreenButton>();
    }
    public bool isPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        button.OnPointerUp(eventData);
    }
}
