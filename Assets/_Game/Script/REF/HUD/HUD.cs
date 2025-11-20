using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class HUD<K> : SingletonMonoBehaviour<K> where K : MonoBehaviour
{

    [Tooltip("If the master background is a button, this allow user click to background to hide popup")]
    // [SerializeField] Button mainBackground;
    [SerializeField] protected Image mainBackground;
    [SerializeField] private bool useMainBackgroundAnim;
    [SerializeField] private float mainBackgroundDuration = 0.2f;
    Button mainBackgroundBtn;
    [Tooltip("Change this size to 0 will reload all frames childs automaticaly")]
    [SerializeField] Frame[] frames;

    List<Frame> activings = new List<Frame>();

    public bool HasActiving => activings.Count > 0;
    public bool IsBusy { get; private set; }

    private float backgroundAlpha = -1;

    private void OnValidate()
    {
        BindAllFrame();
    }

    [ContextMenu("BindAllFrame")]
    protected void ForceBind()
    {
        frames = GetComponentsInChildren<Frame>(true);
        if (frames.Length == 0)
        {
            Debug.LogError($"{name} No one frame was assign!");
        }
    }

    private void BindAllFrame()
    {
        if (frames == null || frames.Length == 0)
        {
            ForceBind();
        }


    }

    private void Start()
    {
        mainBackgroundBtn = mainBackground?.GetComponent<Button>();
        if (mainBackgroundBtn) mainBackgroundBtn.onClick.AddListener(OnBack);
    }

    private void StartBusy() => IsBusy = true;
    private void EndBusy() => IsBusy = false;

    public void ShowPage<T>(bool fromLeft, Action callback = null) where T : Page
    {
        Show<T>(fromLeft, callback);
    }

    public void HidePage<T>(bool toLeft, Action callback = null) where T : Page
    {
        Hide<T>(toLeft, callback);
    }

    public void Show<T>(bool anim = true, Action callback = null, bool hideCurrent = false) where T : Frame
    {
        var frame = Get<T>();
        if (frame == null)
        {
            callback?.Invoke();
            return;
        }

        if (activings.Contains(frame))
        {
            if (frame.isActiveAndEnabled)
            {
                callback?.Invoke();
                return;
            }
            activings.Remove(frame);
        }

        if (hideCurrent) HideCurrent(anim, restorePrevious: true);

        StartBusy();
        activings.Add(frame);
        frame.Show(anim, callback: callback);

        if (mainBackground)
        {
            mainBackground.gameObject.SetActive(true);
            if (useMainBackgroundAnim)
            {
                if (backgroundAlpha < 0)
                {
                    backgroundAlpha = mainBackground.color.a;
                }
                if (backgroundAlpha > 0)
                {
                    mainBackground.DOKill();
                    mainBackground.DOFade(backgroundAlpha, mainBackgroundDuration).From(0).SetUpdate(true).OnComplete(() =>
                    {
                        if (mainBackgroundBtn) mainBackgroundBtn.interactable = frame.HideOnTapBg;
                        EndBusy();
                    });
                }
            }
            else
            {
                if (mainBackgroundBtn) mainBackgroundBtn.interactable = frame.HideOnTapBg;
                EndBusy();
            }
        }
        else EndBusy();
    }

    public void Hide<T>(bool anim = true, Action callback = null) where T : Frame
    {
        var frame = Get<T>();
        if (frame == null)
        {
            callback?.Invoke();
            return;
        }

        if (activings.Contains(frame))
        {
            activings.Remove(frame);
        }

        if (!frame.gameObject.activeSelf)
        {
            callback?.Invoke();
            return;
        }

        callback += EndBusy;
        frame.Hide(anim, callback);

        // has no one previous to restore, so turn off black background
        if (activings.Count == 0 && mainBackground && mainBackground.gameObject.activeInHierarchy)
        {
            if (anim && backgroundAlpha > 0)
            {
                mainBackground.DOKill();
                mainBackground.DOFade(0, .1f).SetUpdate(true).OnComplete(() =>
                {
                    mainBackground.gameObject.SetActive(false);
                    EndBusy();
                });
            }
            else
            {
                mainBackground.gameObject.SetActive(false);
                EndBusy();
            }
        }
    }

    public void HideCurrent(bool anim = true, Action calback = null, bool restorePrevious = true)
    {
        if (!HasActiving)
        {
            Debug.Log("Nothing activing to Hide!");
            calback?.Invoke();
            return;
        }
        StartBusy();
        activings[activings.Count - 1].Hide(anim, calback);
        activings.RemoveAt(activings.Count - 1);

        if (restorePrevious && HasActiving)
        {
            activings[activings.Count - 1].Show(anim, EndBusy);
        }
        else
        {
            // has no one previous to restore, so turn off black background
            if (mainBackground && mainBackground.gameObject.activeInHierarchy)
            {
                if (anim && backgroundAlpha > 0)
                {
                    mainBackground.DOKill();
                    mainBackground.DOFade(0, .1f).SetUpdate(true).OnComplete(() =>
                    {
                        mainBackground.gameObject.SetActive(false);
                        EndBusy();
                    });
                }
                else
                {
                    mainBackground.gameObject.SetActive(false);
                    EndBusy();
                }
            }
            else EndBusy();
        }
    }

    public void HideAll(bool anim, Action hideAction = null)
    {
        activings.Clear();
        Array.ForEach(frames, (f) => f.Hide(anim));
        EndBusy();
        hideAction?.Invoke();
    }

    public T Get<T>() where T : Frame
    {
        var t = Array.Find(frames, (f) => f is T);
        if (t != null) return (T)t;

        Debug.LogError($"{name} Type {typeof(T)} was not assign!");
        return null;
    }

    private void OnBack()
    {
        HideCurrent();
    }

    private void Update()
    {
        if (!IsBusy && Input.GetKeyUp(KeyCode.Escape))
        {
            OnBack();
        }
    }
}

public abstract class Page : Frame
{
    /// <summary> DO NOT USE THIS. Call via HUD.Instance.Show instead.</summary>
    public override void Show(bool fromLeft = true, Action callback = null)
    {
        if (gameObject.activeSelf) return;

        gameObject.SetActive(true);
        if (bg != null)
        {
            if (bgAlpha < 0) bgAlpha = bg.color.a;
            bg.DOKill();
            bg.DOFade(bgAlpha, animTime).From(0);
        }

        MainFrame.DOKill();
        (MainFrame as RectTransform).DOAnchorPosX(0, animTime).From(new Vector2(fromLeft ? -1080 : 1080, 0))
            .SetUpdate(true).SetEase(showType).OnComplete(() =>
            {
                if (callback != null) callback.Invoke();
            });
    }

    /// <summary> DO NOT USE THIS. Call via HUD.Instance.Show instead.</summary>
    public override void Hide(bool toLeft = true, Action callback = null)
    {
        if (!gameObject.activeSelf) return;

        if (bg)
        {
            if (bgAlpha < 0) bgAlpha = bg.color.a;
            bg.DOKill();
            bg.DOFade(0, animTime);
        }

        MainFrame.DOKill();
        (MainFrame as RectTransform).DOAnchorPosX(toLeft ? -1080 : 1080, animTime)
            .SetUpdate(true).SetEase(showType).OnComplete(() =>
            {
                if (callback != null) callback.Invoke();
                gameObject.SetActive(false);
            });
    }
}

public class Frame : MonoBehaviour, IFrame
{
    [SerializeField] private bool hideOnTapBg = false;
    [Tooltip("If turn on hideOnTapBg, this bg need to turn off RaycastTarget")]
    [SerializeField] protected Image bg;
    [SerializeField] Transform mainFrame;
    [SerializeField] protected Ease showType = Ease.OutBack;
    [SerializeField] protected Ease hideType = Ease.InBack;
    [SerializeField] protected float animTime = 0.3f;

    protected float bgAlpha = -1;
    protected Transform MainFrame
    {
        get
        {
            if (mainFrame != null) return mainFrame;
            mainFrame = transform.childCount == 1 ? transform.GetChild(0) : transform;
            return mainFrame;
        }
    }

    public float AnimDuration => animTime;
    public bool HideOnTapBg => hideOnTapBg;

    /// <summary> DO NOT USE THIS. Call via HUD.Instance.Show instead.</summary>
    public virtual void Show(bool animate = true, Action callback = null)
    {
        if (gameObject.activeSelf) return;
        gameObject.SetActive(true);
        if (animate)
        {
            if (bg)
            {
                if (bgAlpha < 0) bgAlpha = bg.color.a;
                bg.DOKill();
                bg.DOFade(bgAlpha, animTime).From(0);
            }
            MainFrame.DOKill();
            MainFrame.DOScale(1, animTime).From(0.5f).SetUpdate(true).SetEase(showType).OnComplete(() =>
            {
                if (callback != null) callback.Invoke();
            });
        }
        else
        {
            MainFrame.localScale = Vector3.one;
            if (callback != null) callback.Invoke();
        }
    }

    /// <summary> DO NOT USE THIS. Call via HUD.Instance.Show instead.</summary>
    public virtual void Hide(bool animate = true, Action callback = null)
    {
        if (!gameObject.activeSelf) return;
        if (animate)
        {
            if (bg)
            {
                if (bgAlpha < 0) bgAlpha = bg.color.a;
                bg.DOKill();
                bg.DOFade(0, animTime);
            }
            MainFrame.DOKill();
            MainFrame.DOScale(.5f, animTime).SetUpdate(true).SetEase(hideType).OnComplete(() =>
            {
                gameObject.SetActive(false);
                if (callback != null) callback.Invoke();
            });
        }
        else
        {
            gameObject.SetActive(false);
            if (callback != null) callback.Invoke();
        }
    }
}

public interface IFrame
{
    void Show(bool animate = true, Action callback = null);
    void Hide(bool animate = true, Action callback = null);
}
