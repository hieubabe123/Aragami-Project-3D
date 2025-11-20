using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// --- CÁC LỚP VÀ ENUM MỚI, HOÀN TOÀN ĐỘC LẬP CHO GAMEDATA ---

/// <summary>
/// (GameData) Enum để định danh các bộ phận cơ thể của người chơi.
/// </summary>
public enum PlayerBodyPartType
{
    Eye,
    Body,
    Mouth,
}

/// <summary>
/// (GameData) Liên kết một bộ phận cơ thể với một hoặc nhiều material.
/// </summary>
[System.Serializable]
public class PlayerBodyPartMaterial
{
    [Tooltip("Loại bộ phận cơ thể")]
    public PlayerBodyPartType PartType;

    [Tooltip("Danh sách các material có thể áp dụng cho bộ phận này.")]
    public List<Material> Materials = new List<Material>();

    public Material GetRandomMaterial()
    {
        if (Materials.Count == 0) return null;
        if (Materials.Count == 1) return Materials[0];
        int randomIndex = Random.Range(0, Materials.Count);
        return Materials[randomIndex];
    }
}

/// <summary>
/// (GameData) Đại diện cho một bộ trang phục hoàn chỉnh của người chơi.
/// </summary>
[System.Serializable]
public class PlayerAppearanceSet
{
    [Tooltip("Tên của bộ trang phục")]
    public string SetName;

    [Tooltip("Đánh dấu đây là bộ trang phục mặc định")]
    public bool IsDefault = false;

    [Tooltip("Danh sách các material cho từng bộ phận trong bộ trang phục này")]
    public List<PlayerBodyPartMaterial> PartMaterials = new List<PlayerBodyPartMaterial>();

    /// <summary>
    /// Gộp tất cả material từ các bộ phận lại thành một mảng duy nhất.
    /// Dùng để tương thích với các hàm cũ.
    /// </summary>
    public Material[] GetAllMaterials()
    {
        if (PartMaterials == null) return new Material[0];
        return PartMaterials.SelectMany(p => p.Materials).ToArray();
    }
}

/// <summary>
/// (GameData) Cấu trúc dữ liệu mới cho một nhân vật người chơi.
/// </summary>
[System.Serializable]
public class PlayerCharacterData
{
    [Tooltip("ID duy nhất và không đổi của nhân vật. Dùng để so khớp với Firebase.")]
    public int CharacterId;
    [Tooltip("Tên để dễ nhận biết")]
    public string CharacterName;

    [Tooltip("Prefab model 3D của nhân vật")]
    public GameObject Prefab3D;

    [Tooltip("Icon hiển thị trong cửa hàng")]
    public Sprite ShopIcon2D;

    [Tooltip("Giá bán trong cửa hàng")]
    public int ShopPrice;

    [Tooltip("Danh sách tất cả các bộ trang phục của nhân vật này")]
    public List<PlayerAppearanceSet> AppearanceSets = new List<PlayerAppearanceSet>();

    [HideInInspector] public int Idx;

    public PlayerAppearanceSet GetDefaultAppearanceSet()
    {
        return AppearanceSets.FirstOrDefault(s => s.IsDefault) ?? AppearanceSets.FirstOrDefault();
    }
}


// --- SCRIPT GAMEDATA CHÍNH ---

[CreateAssetMenu(fileName = "GameData", menuName = "Data/GameData")]
public class GameData : SingletonResources<GameData>
{

    [Header("Game Settings")]
    public int coinRewardedByAd = 2000;

    [Header("Player Character Database")]
    [Tooltip("Cấu hình nhân vật người chơi ở đây")]
    public List<PlayerCharacterData> PlayerCharacters = new List<PlayerCharacterData>();
    public int CharacterLength => PlayerCharacters.Count;

    public void SortCharacterByOrderIds(List<int> orderIds)
    {
        if (orderIds == null || orderIds.Count == 0) return;

        var characterDict = PlayerCharacters.ToDictionary(c => c.CharacterId, c => c);
        var sortedCharacters = new List<PlayerCharacterData>();

        // Thêm các nhân vật theo thứ tự từ Firebase
        foreach (int id in orderIds)
        {
            if (characterDict.TryGetValue(id, out var character))
            {
                sortedCharacters.Add(character);
                characterDict.Remove(id);
            }
        }

        sortedCharacters.AddRange(characterDict.Values);
        PlayerCharacters = sortedCharacters;

        OnValidate();
    }
    public GameObject GetCharacterModel(int idx)
    {
        if (idx < 0 || idx >= CharacterLength) return null;
        return PlayerCharacters[idx].Prefab3D;
    }

    public Character GetCharacter(int idx)
    {
        if (idx < 0 || idx >= CharacterLength) return null;

        PlayerCharacterData newCharData = PlayerCharacters[idx];
        PlayerAppearanceSet defaultSet = newCharData.GetDefaultAppearanceSet();

        return new Character
        {
            Prefab3D = newCharData.Prefab3D,
            ShopIcon2D = newCharData.ShopIcon2D,
            ShopPrice = newCharData.ShopPrice,
            Idx = idx,
            DefaultClothSet = new ClothSet
            {
                Name = defaultSet?.SetName ?? "Default",
                CharacterMaterials = defaultSet?.GetAllMaterials() ?? new Material[0]
            }
        };
    }

    public Material[] GetClothes(int characterIdx)
    {
        if (characterIdx < 0 || characterIdx >= CharacterLength) return null;

        PlayerAppearanceSet defaultSet = PlayerCharacters[characterIdx].GetDefaultAppearanceSet();
        return defaultSet?.GetAllMaterials() ?? new Material[0];
    }

    public CharacterWithClothes GetRandomCharacter()
    {
        if (CharacterLength == 0) return new CharacterWithClothes();

        int randomIdx = Random.Range(0, CharacterLength);
        PlayerCharacterData randomChar = PlayerCharacters[randomIdx];
        PlayerAppearanceSet defaultSet = randomChar.GetDefaultAppearanceSet();

        return new CharacterWithClothes
        {
            CharacterPrefab = randomChar.Prefab3D,
            ClothMaterials = defaultSet?.GetAllMaterials() ?? new Material[0]
        };
    }

    private void OnValidate()
    {
        for (int i = 0; i < PlayerCharacters.Count; i++)
        {
            PlayerCharacters[i].Idx = i;
        }
        ValidateCharacterIds();
    }

    private void ValidateCharacterIds()
    {
        var duplicateGroups = PlayerCharacters
            .Where(c => c != null)
            .GroupBy(c => c.CharacterId)
            .Where(g => g.Count() > 1);

        if (duplicateGroups.Any())
        {
            foreach (var group in duplicateGroups)
            {
                Debug.LogError($"LỖI VALIDATE GAMEDATA: CharacterId '{group.Key}' đang bị sử dụng trùng lặp. Vui lòng sửa lại trong Inspector.", this);
            }
        }
    }
}


// --- CÁC LỚP CŨ ĐƯỢC GIỮ LẠI ĐỂ ĐẢM BẢO TƯƠNG THÍCH ---

[System.Serializable]
public class Character
{
    public GameObject Prefab3D;
    public Sprite ShopIcon2D;
    public int ShopPrice;
    public ClothSet DefaultClothSet;
    [HideInInspector] public int Idx;
}

[System.Serializable]
public class ClothSet
{
    public string Name;
    public Material[] CharacterMaterials;
}

public struct CharacterWithClothes
{
    public GameObject CharacterPrefab;
    public Material[] ClothMaterials;
}
