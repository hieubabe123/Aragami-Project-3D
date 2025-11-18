using UnityEngine;
using UnityEngine.UI;

public class PlayerGUIView : MonoBehaviour
{
    [SerializeField] private Button runBtn;
    [SerializeField] private Button stealKillBtn;
    [SerializeField] private Button createShadowBtn;
    [SerializeField] private Button teleportBtn;

    void Start()
    {
        stealKillBtn.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        EventDispatcher.AddListener<EventDefine.OnPlayerCanStealthKill>(OnPlayerCanStealthKill);
    }

    void OnDisable()
    {
        EventDispatcher.RemoveListener<EventDefine.OnPlayerCanStealthKill>(OnPlayerCanStealthKill);
    }

    private void OnPlayerCanStealthKill(EventDefine.OnPlayerCanStealthKill evt)
    {
        stealKillBtn.gameObject.SetActive(evt.CanStealthKill);
    }
}
