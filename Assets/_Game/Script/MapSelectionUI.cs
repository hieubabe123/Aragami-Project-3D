using UnityEngine;
using UnityEngine.UI;

public class MapSelectionUI : MonoBehaviour
{
    [SerializeField] private string mapID;
    private Button mapChosenBtn;

    private void Awake()
    {
        mapChosenBtn = GetComponent<Button>();
    }

    private void OnEnable()
    {
        mapChosenBtn.onClick.AddListener(OnMapChosenClicked);
    }

    private void OnDisable()
    {
        mapChosenBtn.onClick.RemoveListener(OnMapChosenClicked);
    }

    private void OnMapChosenClicked()
    {
        EventDispatcher.Dispatch(new EventDefine.OnChangeScene() { SceneName = mapID });
    }
}
