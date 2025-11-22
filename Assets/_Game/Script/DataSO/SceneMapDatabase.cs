using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneMapDatabase", menuName = "GameData/Scene Map Database")]
public class SceneMapDatabase : ScriptableObject
{
    [System.Serializable]
    public struct SceneMapMapping
    {
        public int mapID;
        public string sceneName; // Tên của scene tương ứng, ví dụ: "Map_1", "Level_Forest"
    }

    public List<SceneMapMapping> sceneMaps;

    /// <summary>
    /// Lấy tên scene từ ID của map.
    /// </summary>
    public string GetSceneNameByID(int id)
    {
        var mapping = sceneMaps.FirstOrDefault(m => m.mapID == id);
        return mapping.sceneName; // Sẽ trả về null nếu không tìm thấy
    }

    /// <summary>
    /// Lấy ID của map tiếp theo trong danh sách.
    /// </summary>
    public int GetNextMapID(int currentMapID)
    {
        int currentIndex = sceneMaps.FindIndex(m => m.mapID == currentMapID);

        if (currentIndex != -1 && currentIndex < sceneMaps.Count - 1)
        {
            return sceneMaps[currentIndex + 1].mapID;
        }

        return -1; // Không có map tiếp theo
    }
}
