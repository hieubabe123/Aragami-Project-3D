using UnityEngine;

public class EnemyIcons : MonoBehaviour
{
    [SerializeField] private GameObject questionMarkIcon;
    [SerializeField] private GameObject exclamationMarkIcon;
    [SerializeField] private GameObject attackMarkIcon;
    [SerializeField] private GameObject alertAllEnemyIcon;

    void Start()
    {
        HideAllIcons();
    }

    public void ShowQuestionMarkIcon()
    {
        questionMarkIcon.SetActive(true);
        exclamationMarkIcon.SetActive(false);
        attackMarkIcon.SetActive(false);
        alertAllEnemyIcon.SetActive(false);
    }

    public void ShowExclamationMarkIcon()
    {
        questionMarkIcon.SetActive(false);
        exclamationMarkIcon.SetActive(true);
        attackMarkIcon.SetActive(false);
        alertAllEnemyIcon.SetActive(false);

    }

    public void ShowAttackMarkIcon()
    {
        questionMarkIcon.SetActive(false);
        exclamationMarkIcon.SetActive(false);
        attackMarkIcon.SetActive(true);
        alertAllEnemyIcon.SetActive(false);

    }

    public void ShowAlertAllEnemyIcon()
    {
        questionMarkIcon.SetActive(false);
        exclamationMarkIcon.SetActive(false);
        attackMarkIcon.SetActive(false);
        alertAllEnemyIcon.SetActive(true);
    }

    public void HideAllIcons()
    {
        alertAllEnemyIcon.SetActive(false);
        attackMarkIcon.SetActive(false);
        questionMarkIcon.SetActive(false);
        exclamationMarkIcon.SetActive(false);
    }
}
