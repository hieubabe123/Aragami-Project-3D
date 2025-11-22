using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    public void OnContinueClicked()
    {
        // Continue with current chap in UserData
        EventDispatcher.Dispatch(new EventDefine.OnChangeScene { SceneName = GameScene.Gameplay });
    }

    public void OnStoryClicked()
    {
        PopupManager.Instance.Show<StoryPopup>();
    }

    public void OnMissionClicked()
    {

    }

    public void OnSkinClicked()
    {

    }

    public void OnSettingsClicked()
    {
        PopupManager.Instance.Show<SettingPopup>();
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }



}
