using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum ButtonMenuType
{
    ContinueBtn,
    StoryBtn,
    MissionBtn,
    SkinBtn,
    OptionBtn,
    QuitBtn
}
public class ButtonMenu : MonoBehaviour
{
    [Header("------------------Sprites------------------")]
    [SerializeField] private MenuController menuController;
    public Sprite selectedButton;
    public Sprite unselectedButton;
    private Image image;
    public ButtonMenuType buttonType;
    public bool isSelected;

    public static ButtonMenu instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    void Start()
    {
        isSelected = false;
        image.sprite = unselectedButton;
    }

    public void OnClickSelectMap()
    {
        if (instance != null && instance != this)
        {
            instance.Unselect();
        }

        if (!isSelected)
        {
            Select();
        }
        else
        {
            switch (buttonType)
            {
                case ButtonMenuType.ContinueBtn:
                    menuController.OnContinueClicked();
                    break;
                case ButtonMenuType.StoryBtn:
                    menuController.OnStoryClicked();
                    break;
                case ButtonMenuType.MissionBtn:
                    menuController.OnMissionClicked();
                    break;
                case ButtonMenuType.SkinBtn:
                    menuController.OnSkinClicked();
                    break;
                case ButtonMenuType.OptionBtn:
                    menuController.OnSettingsClicked();
                    break;
                case ButtonMenuType.QuitBtn:
                    menuController.OnQuitClicked();
                    break;
                default:
                    break;
            }
        }
    }
    private void Unselect()
    {
        isSelected = false;
        image.sprite = unselectedButton;
        image.color = Color.black;
    }
    private void Select()
    {
        instance = this;
        isSelected = true;
        image.sprite = selectedButton;
        image.color = Color.white;
    }
}
