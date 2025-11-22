using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup winCanvasGroup;
    [SerializeField] private Button nextGameBtn;
    [SerializeField] private Button homeBtn;

    void Start()
    {
        SetCanvasVisibility(false);
        EventDispatcher.AddListener<EventDefine.OnWin>(OnWin);
        nextGameBtn.onClick.AddListener(OnNextGameClicked);
        homeBtn.onClick.AddListener(OnHomeClicked);
    }

    void OnDestroy()
    {
        EventDispatcher.RemoveListener<EventDefine.OnWin>(OnWin);
        nextGameBtn.onClick.RemoveListener(OnNextGameClicked);
        homeBtn.onClick.RemoveListener(OnHomeClicked);
    }

    private void OnNextGameClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadNextMap();
        }
    }

    private void OnWin(EventDefine.OnWin evt)
    {
        ShowWinUI();
    }

    private void OnHomeClicked()
    {
        ChangeScene(GameScene.Menu);
    }

    private void SetCanvasVisibility(bool isVisible)
    {
        winCanvasGroup.alpha = isVisible ? 1.0f : 0.0f;
        winCanvasGroup.blocksRaycasts = isVisible;
    }

    private void ChangeScene(string sceneName)
    {
        EventDispatcher.Dispatch(new EventDefine.OnChangeScene() { SceneName = sceneName });
    }

    private void ShowWinUI()
    {
        winCanvasGroup.DOFade(1f, 0.3f).From(0).SetEase(Ease.Linear);
        winCanvasGroup.blocksRaycasts = true;
    }
}
