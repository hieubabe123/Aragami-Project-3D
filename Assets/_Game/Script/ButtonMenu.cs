using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMenu : MonoBehaviour
{
    [Header("------------------Sprites------------------")]
    public Sprite selectedButton;
    public Sprite unselectedButton;
    private Image image;
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
            //DO SOMETHING IF ALREADY SELECTED
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
