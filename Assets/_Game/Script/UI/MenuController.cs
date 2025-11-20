using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button storyBtn;
    [SerializeField] private Button missionBtn;
    [SerializeField] private Button skinBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button quitBtn;

    private void Start()
    {
        continueBtn.onClick.AddListener(OnContinueClicked);
        storyBtn.onClick.AddListener(OnStoryClicked);
        missionBtn.onClick.AddListener(OnMissionClicked);
        skinBtn.onClick.AddListener(OnSkinClicked);
        settingsBtn.onClick.AddListener(OnSettingsClicked);
        quitBtn.onClick.AddListener(OnQuitClicked);
    }

    void OnDestroy()
    {
        continueBtn.onClick.RemoveListener(OnContinueClicked);
        storyBtn.onClick.RemoveListener(OnStoryClicked);
        missionBtn.onClick.RemoveListener(OnMissionClicked);
        skinBtn.onClick.RemoveListener(OnSkinClicked);
        settingsBtn.onClick.RemoveListener(OnSettingsClicked);
        quitBtn.onClick.RemoveListener(OnQuitClicked);
    }

    private void OnContinueClicked()
    {
        // Continue with current chap in UserData
        EventDispatcher.Dispatch(new EventDefine.OnChangeScene { SceneName = GameScene.Gameplay });
    }

    private void OnStoryClicked()
    {

    }

    private void OnMissionClicked()
    {

    }

    private void OnSkinClicked()
    {

    }

    private void OnSettingsClicked()
    {

    }

    private void OnQuitClicked()
    {
        Application.Quit();
    }



}
