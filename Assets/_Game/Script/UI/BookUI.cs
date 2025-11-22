using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI curerntBookText;
    private int currentBookCout = 0;
    private int maxBookCount = 3;

    private void Update()
    {
        if (currentBookCout >= maxBookCount)
        {
            EventDispatcher.Dispatch<EventDefine.OnWin>();
        }
    }

    private void OnEnable()
    {
        EventDispatcher.AddListener<EventDefine.OnPlayerPickBook>(OnPlayerPickBook);
        EventDispatcher.AddListener<EventDefine.OnWin>(OnWin);
    }

    private void OnDisable()
    {
        EventDispatcher.RemoveListener<EventDefine.OnPlayerPickBook>(OnPlayerPickBook);
        EventDispatcher.RemoveListener<EventDefine.OnWin>(OnWin);
    }

    private void OnPlayerPickBook(EventDefine.OnPlayerPickBook evt)
    {
        UpdateBookUI();
    }

    private void OnWin(EventDefine.OnWin evt)
    {
        int currentMapId = GameManager.Instance.mapName;
        if (!UserData.IsMapCompleted(currentMapId))
        {
            UserData.CurrentBook += currentBookCout;
            Debug.Log("Map not completed. Saving book progress. Total books: " + UserData.CurrentBook);
        }
        else
        {
            Debug.Log("Map already completed. Book count for this session increased, but not saved permanently.");
        }
    }

    private void UpdateBookUI()
    {
        currentBookCout++;
        curerntBookText.text = currentBookCout.ToString();
    }
}
