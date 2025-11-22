using System.Collections.Generic;
using UnityEngine;

// Bỏ SingletonMonoBehaviour và dùng MonoBehaviour thông thường
public class GameManager : MonoBehaviour
{
    // Thêm một biến static để các script khác có thể truy cập dễ dàng trong cùng một scene
    public static GameManager Instance { get; private set; }

    public int mapName;
    [SerializeField] private SceneMapDatabase sceneMapDatabase; // Kéo asset vào đây

    private void Awake()
    {
        // Thiết lập instance cho scene hiện tại
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        mapName = UserData.CurrentMap;
    }

    void OnEnable()
    {
        EventDispatcher.AddListener<EventDefine.OnWin>(OnWin);
        EventDispatcher.AddListener<EventDefine.OnPlayerDead>(OnPlayerDead);
    }

    void OnDisable()
    {
        EventDispatcher.RemoveListener<EventDefine.OnWin>(OnWin);
        EventDispatcher.RemoveListener<EventDefine.OnPlayerDead>(OnPlayerDead);
    }

    private void OnWin(EventDefine.OnWin evt)
    {
        List<int> completedMaps = UserData.CompletedMaps;
        if (!completedMaps.Contains(mapName))
        {
            completedMaps.Add(mapName);
            UserData.CompletedMaps = completedMaps;

            Debug.Log($"Map {mapName} completed and saved!");
        }
        else
        {
            Debug.Log($"Map {mapName} was already completed. Progress not saved again.");
        }
    }

    private void OnPlayerDead(EventDefine.OnPlayerDead evt)
    {

    }

    public void LoadNextMap()
    {
        int nextMapID = sceneMapDatabase.GetNextMapID(mapName);

        if (nextMapID != -1)
        {
            // Nếu có màn chơi tiếp theo, lưu nó làm màn chơi hiện tại
            UserData.CurrentMap = nextMapID;
            // Lấy tên scene từ database
            string nextSceneName = sceneMapDatabase.GetSceneNameByID(nextMapID);
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                // Gửi sự kiện để tải scene tiếp theo
                EventDispatcher.Dispatch(new EventDefine.OnChangeScene { SceneName = nextSceneName });
            }
            else
            {
                Debug.LogError($"Không tìm thấy tên scene cho Map ID: {nextMapID}");
            }
        }
        else
        {
            // Nếu không còn màn chơi nào, quay về Menu
            Debug.Log("Đã hoàn thành tất cả các màn chơi! Quay về Menu.");
            EventDispatcher.Dispatch(new EventDefine.OnChangeScene { SceneName = GameScene.Menu });
        }
    }
}



[System.Serializable]
public struct GameScene
{
    public static string Menu = "Menu";
    public static string Gameplay = "Gameplay";
    public static string Gameplay_1 = "Gameplay_1";
    public static string Gameplay_2 = "Gameplay_2";


}
