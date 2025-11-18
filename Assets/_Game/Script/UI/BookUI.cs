using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI curerntBookText;
    private int maxBookCount = 3;

    private void OnEnable()
    {
        EventDispatcher.AddListener<EventDefine.OnPlayerPickBook>(OnPlayerPickBook);
    }

    private void OnDisable()
    {
        EventDispatcher.RemoveListener<EventDefine.OnPlayerPickBook>(OnPlayerPickBook);
    }

    private void OnPlayerPickBook(EventDefine.OnPlayerPickBook evt)
    {
        UpdateBookUI();
    }

    private void UpdateBookUI()
    {

    }
}
