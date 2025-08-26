using UnityEngine;

public class EnemyIcons : MonoBehaviour
{
    [SerializeField] private GameObject questionMarkIcon;
    [SerializeField] private GameObject exclamationMarkIcon;
    [SerializeField] private GameObject attackMarkIcon;

    void Start()
    {
        HideAllIcons();
    }

    public void ShowQuestionMarkIcon()
    {
        questionMarkIcon.SetActive(true);
        exclamationMarkIcon.SetActive(false);
        attackMarkIcon.SetActive(false);
    }

    public void ShowExclamationMarkIcon()
    {
        questionMarkIcon.SetActive(false);
        exclamationMarkIcon.SetActive(true);
        attackMarkIcon.SetActive(false);
    }

    public void ShowAttackMarkIcon()
    {
        questionMarkIcon.SetActive(false);
        exclamationMarkIcon.SetActive(false);
        attackMarkIcon.SetActive(true);
    }

    public void HideAllIcons()
    {

        questionMarkIcon.SetActive(false);
        exclamationMarkIcon.SetActive(false);
    }
}
