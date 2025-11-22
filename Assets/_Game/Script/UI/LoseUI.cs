using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoseUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup winCanvasGroup;
    [SerializeField] private Button replayBtn;
    [SerializeField] private Button homeBtn;

    void Start()
    {
        SetCanvasVisibility(false);
        EventDispatcher.AddListener<EventDefine.OnPlayerDead>(OnPlayerDead);
        replayBtn.onClick.AddListener(OnReplayBtn);
        homeBtn.onClick.AddListener(OnHomeClicked);
    }

    void OnDestroy()
    {
        EventDispatcher.RemoveListener<EventDefine.OnPlayerDead>(OnPlayerDead);
        replayBtn.onClick.RemoveListener(OnReplayBtn);
        homeBtn.onClick.RemoveListener(OnHomeClicked);
    }

    private void OnReplayBtn()
    {
        ChangeScene(SceneManager.GetActiveScene().name);
    }

    private void OnPlayerDead(EventDefine.OnPlayerDead evt)
    {
        ShowLoseUI();
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

    private void ShowLoseUI()
    {
        winCanvasGroup.DOFade(1f, 0.3f).From(0).SetEase(Ease.Linear);
        winCanvasGroup.blocksRaycasts = true;
    }
}
