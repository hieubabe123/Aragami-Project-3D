using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// <summary>
/// Enum để định danh các bộ phận cơ thể.
/// Enum này phải khớp với những gì bạn thiết lập trong ModelSkinCtrl.
/// Bạn có thể thêm/sửa các bộ phận này tùy theo model của bạn.
/// </summary>
public enum BodyPartType
{
    Eye,
    Body,
    Mouth,
}

[CreateAssetMenu(fileName = "BotData", menuName = "456-Mini-Games/Bot Data", order = 1)]
public class BotData : SingletonResources<BotData>
{
    [Header("Character Database")]
    [Tooltip("Danh sách tất cả các nhân vật có thể spawn trong game")]
    public List<BotCharacter> BotCharacters = new List<BotCharacter>();

    public int CharacterCount => BotCharacters.Count;

    /// <summary>
    /// Lấy một nhân vật ngẫu nhiên từ danh sách.
    /// </summary>
    public BotCharacter GetRandomCharacter()
    {
        if (CharacterCount == 0)
        {
            Debug.LogError("BotData has no characters configured!", this);
            return null;
        }
        int randomIndex = Random.Range(0, CharacterCount);
        return BotCharacters[randomIndex];
    }

    /// <summary>
    /// Lấy một nhân vật cụ thể bằng index.
    /// </summary>
    public BotCharacter GetCharacter(int index)
    {
        if (index < 0 || index >= CharacterCount)
        {
            Debug.LogError($"Invalid character index: {index}. Valid range is 0 to {CharacterCount - 1}.", this);
            return null;
        }
        return BotCharacters[index];
    }
}

/// <summary>
/// Đại diện cho một nhân vật, chứa model và các bộ trang phục có thể có.
/// </summary>
[System.Serializable]
public class BotCharacter
{
    [Tooltip("Tên để dễ nhận biết trong Inspector")]
    public string CharacterName;

    [Tooltip("Prefab model 3D của nhân vật. Prefab này phải có script ModelSkinCtrl.")]
    public GameObject Prefab3D;

    [Tooltip("Danh sách tất cả các bộ trang phục có thể có cho nhân vật này")]
    public List<CharacterAppearanceSet> AppearanceSets = new List<CharacterAppearanceSet>();

    /// <summary>
    /// Lấy một bộ trang phục ngẫu nhiên từ danh sách của nhân vật này.
    /// </summary>
    public CharacterAppearanceSet GetRandomAppearanceSet()
    {
        if (AppearanceSets.Count == 0)
        {
            Debug.LogWarning($"Character '{CharacterName}' has no appearance sets configured.", Prefab3D);
            return null;
        }
        int randomIndex = Random.Range(0, AppearanceSets.Count);
        return AppearanceSets[randomIndex];
    }
}

/// <summary>
/// Đại diện cho một bộ trang phục hoàn chỉnh (ví dụ: "Bộ đồ cảnh sát", "Bộ đồ thể thao").
/// Đây là đối tượng được truyền vào hàm ModelSkinCtrl.ApplyAppearance().
/// </summary>
[System.Serializable]
public class CharacterAppearanceSet
{
    [Tooltip("Tên của bộ trang phục, ví dụ: 'Casual Outfit'")]
    public string SetName;

    [Tooltip("Danh sách các material cho từng bộ phận trong bộ trang phục này")]
    public List<BodyPartMaterial> PartMaterials = new List<BodyPartMaterial>();

    /// <summary>
    /// Lấy material cho một bộ phận cụ thể trong bộ trang phục này.
    /// </summary>
    public Material GetMaterialForPart(BodyPartType partType)
    {
        // Tìm bộ phận tương ứng trong danh sách
        var part = PartMaterials.FirstOrDefault(p => p.PartType == partType);
        if (part != null)
        {
            // Nếu tìm thấy, lấy một material ngẫu nhiên từ đó
            return part.GetRandomMaterial();
        }
        // Trả về null nếu không tìm thấy bộ phận này trong bộ trang phục
        return null;
    }
}

/// <summary>
/// Liên kết một bộ phận cơ thể với một hoặc nhiều material có thể có.
/// Điều này cho phép một bộ phận có nhiều biến thể (ví dụ: 5 màu áo khác nhau).
/// </summary>
[System.Serializable]
public class BodyPartMaterial
{
    [Tooltip("Loại bộ phận cơ thể (ví dụ: Đầu, Thân)")]
    public BodyPartType PartType;

    [Tooltip("Danh sách các material có thể áp dụng cho bộ phận này. Sẽ chọn ngẫu nhiên một trong số này.")]
    public List<Material> Materials = new List<Material>();

    /// <summary>
    /// Lấy một material ngẫu nhiên từ danh sách.
    /// </summary>
    public Material GetRandomMaterial()
    {
        if (Materials.Count == 0) return null;
        if (Materials.Count == 1) return Materials[0]; // Tối ưu hóa cho trường hợp chỉ có 1 material
        int randomIndex = Random.Range(0, Materials.Count);
        return Materials[randomIndex];
    }
}

