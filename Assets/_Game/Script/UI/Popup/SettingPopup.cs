using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : Frame
{
    #region SerializeField

    [Header("Buttons")]
    [SerializeField] Button homeBtn;
    [SerializeField] Button closeBtn;
    [Space]
    [SerializeField] Toggle soundToggle;
    [SerializeField] Toggle musicToggle;
    [SerializeField] Toggle vibrationToggle;
    [SerializeField] Slider touchSensitivitySlider;
    #endregion

    #region  Field

    private void Awake()
    {
        soundToggle.isOn = SFX.Instance.SoundEnable;
        musicToggle.isOn = SFX.Instance.MusicEnable;
        vibrationToggle.isOn = SFX.Instance.VibrateEnable;
        if (touchSensitivitySlider != null)
        {
            touchSensitivitySlider.value = UserData.touchSensitivity;
        }

    }
    #endregion
    private void Start()
    {
        homeBtn?.onClick.AddListener(OnHomeClick);
        closeBtn?.onClick.AddListener(OnCloseClick);
    }

    private void OnDestroy()
    {
        homeBtn?.onClick.RemoveListener(OnHomeClick);
        closeBtn?.onClick.RemoveListener(OnCloseClick);
    }


    #region  Public Func
    public override void Show(bool animate = true, Action callback = null)
    {
        EventDispatcher.Dispatch(new EventDefine.OnPause() { IsPause = true });
        base.Show(animate, callback);

    }
    public override void Hide(bool animate = true, Action callback = null)
    {
        EventDispatcher.Dispatch(new EventDefine.OnPause() { IsPause = false });
        base.Hide(animate, callback);
    }
    #endregion
    #region  Func
    public void OnCloseClick()
    {
        // if (GameCtrl.Instance.Playing) this.Dispatch(new EventDefine.OnGamePause { isPaused = false });
        // this.Hide();
        EventDispatcher.Dispatch(new EventDefine.OnPause() { IsPause = false });
        PopupManager.Instance.HideCurrent();
        Debug.Log("Close Setting Popup");
    }

    public void OnHomeClick()
    {
        EventDispatcher.Dispatch(new EventDefine.OnChangeScene() { SceneName = GameScene.Menu });

    }
    #endregion
}
