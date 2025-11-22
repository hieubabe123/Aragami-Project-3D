using System;
using UnityEngine;
using UnityEngine.UI;

public class StoryPopup : Frame
{

    #region SerializeField

    [Header("Buttons")]
    [SerializeField] Button closeBtn;
    #endregion

    #region  Field

    #endregion
    private void Start()
    {
        closeBtn?.onClick.AddListener(OnCloseClick);
    }

    private void OnDestroy()
    {
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
        EventDispatcher.Dispatch(new EventDefine.OnPause() { IsPause = false });
        PopupManager.Instance.HideCurrent();
        Debug.Log("Close Setting Popup");
    }
    #endregion
}
