using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleChange : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] UnityEngine.UI.Toggle toggle;
    [SerializeField] ToggleType thisTogType;
    [SerializeField] Image backgroundImage;

    [SerializeField] GameObject[] backgroundState; //0 : On, 1 : Off
    Transform backgroundCurrentStateTrs; //0 : On, 1 : Off

    private void Start()
    {
        toggle = this.GetComponent<UnityEngine.UI.Toggle>();
        backgroundImage = this.GetComponent<UnityEngine.UI.Image>();

        OnChangeWithoutAnimate(toggle.isOn);
        toggle.onValueChanged.AddListener(OnClick);
        //OnSwitch(toggle.isOn);
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnClick);

    }

    void OnChangeWithoutAnimate(bool on)
    {

        backgroundState[0].gameObject.SetActive(on);

        backgroundState[1].gameObject.SetActive(!on);

        backgroundCurrentStateTrs = backgroundState[on ? 0 : 1].transform;
        SaveData(on);
    }
    //void OnChangeWithAnimate(bool on) {
    //    backgroundState[0].gameObject.SetActive(on);
    //    backgroundState[0].transform.DOScale(0.9f, 0.2f).From().SetEase(Ease.Linear);
    //    SaveData(on);
    //}


    private void SaveData(bool on)
    {
        switch (thisTogType)
        {
            case ToggleType.Sound:
                {
                    SFX.Instance.SoundEnable = on;
                    break;
                }
            case ToggleType.Music:
                {
                    SFX.Instance.MusicEnable = on;
                    break;
                }
            case ToggleType.Vibration:
                {
                    SFX.Instance.VibrateEnable = on;
                    break;
                }
            default:
                break;
        }
    }

    void OnClick(bool on)
    {
        OnChangeWithoutAnimate(on);
        SFX.Instance.PlaySound(GameSoundData.Instance.ButtonClick);

    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (backgroundCurrentStateTrs)
        {
            backgroundCurrentStateTrs.transform.DOScale(0.9f, 0.2f).From(1).SetEase(Ease.Linear);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (backgroundCurrentStateTrs)
        {
            backgroundCurrentStateTrs.transform.DOScale(1f, 0.2f).From(0.9f).SetEase(Ease.Linear);
        }
    }
}

public enum ToggleType
{
    Sound,
    Music,
    Vibration
}
