using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button optionBtn;
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private Button quitBtn;

    void OnEnable()
    {
        resumeBtn?.onClick.AddListener(OnResumeClicked);
        restartBtn?.onClick.AddListener(OnRestartClicked);
        mainMenuBtn?.onClick.AddListener(OnMainMenuClicked);
        optionBtn?.onClick.AddListener(OnOptionClicked);
        quitBtn?.onClick.AddListener(OnQuitClicked);
    }

    void OnDisable()
    {
        resumeBtn?.onClick.RemoveListener(OnResumeClicked);
        restartBtn?.onClick.RemoveListener(OnRestartClicked);
        mainMenuBtn?.onClick.RemoveListener(OnMainMenuClicked);
        optionBtn?.onClick.RemoveListener(OnOptionClicked);
        quitBtn?.onClick.RemoveListener(OnQuitClicked);
    }

    private void OnResumeClicked()
    {
        EventDispatcher.Dispatch(new EventDefine.OnPause() { IsPause = false });
        settingPanel.SetActive(false);
    }

    private void OnRestartClicked()
    {
        EventDispatcher.Dispatch(new EventDefine.OnChangeScene() { SceneName = SceneManager.GetActiveScene().name });
    }

    private void OnMainMenuClicked()
    {
        EventDispatcher.Dispatch(new EventDefine.OnChangeScene() { SceneName = GameScene.Menu });
    }

    private void OnOptionClicked()
    {
        PopupManager.Instance.Show<SettingPopup>();
    }

    private void OnQuitClicked()
    {
        Application.Quit();
    }
}
