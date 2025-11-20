using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public partial class UserData
{

    public static int CurrentBook
    {
        get => GetInt("CurrentBook", 0);
        set => SetInt("CurrentBook", value);
    }

    public static int CurrentChapter
    {
        get => GetInt("CurrentChapter", 0);
        set => SetInt("CurrentChapter", value);
    }
    public static float touchSensitivity
    {
        get => GetFloat("TouchSensitivity", 1f);
        set => SetFloat("TouchSensitivity", value);
    }

}


public partial class UserData
{
    // Integer
    public static void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);
    public static int GetInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(key, defaultValue);

    // Float
    public static void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);
    public static float GetFloat(string key, float defaultValue = 0f) => PlayerPrefs.GetFloat(key, defaultValue);

    // Boolean
    public static void SetBool(string key, bool value) => PlayerPrefs.SetInt(key, value ? 1 : 0);
    public static bool GetBool(string key, bool defaultValue = false) => PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;

    // String
    public static void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
    public static string GetString(string key, string defaultValue = "") => PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key) : defaultValue;
    // Long (Stored as String)
    public static void SetLong(string key, long value) => PlayerPrefs.SetString(key, value.ToString());
    public static long GetLong(string key, long defaultValue = 0) =>
        PlayerPrefs.HasKey(key) && long.TryParse(PlayerPrefs.GetString(key), out long result) ? result : defaultValue;

    // Double (Stored as String)
    public static void SetDouble(string key, double value) => PlayerPrefs.SetString(key, value.ToString("R")); // "R" ensures full precision
    public static double GetDouble(string key, double defaultValue = 0.0) =>
        PlayerPrefs.HasKey(key) && double.TryParse(PlayerPrefs.GetString(key), out double result) ? result : defaultValue;
    // Unity Data Types
    public static void SetVector2(string key, Vector2 value) => SetObject(key, value);
    public static Vector2 GetVector2(string key, Vector2 defaultValue) => GetObject(key, defaultValue);

    public static void SetVector3(string key, Vector3 value) => SetObject(key, value);
    public static Vector3 GetVector3(string key, Vector3 defaultValue) => GetObject(key, defaultValue);

    public static void SetVector4(string key, Vector4 value) => SetObject(key, value);
    public static Vector4 GetVector4(string key, Vector4 defaultValue) => GetObject(key, defaultValue);

    public static void SetQuaternion(string key, Quaternion value) => SetObject(key, value);
    public static Quaternion GetQuaternion(string key, Quaternion defaultValue) => GetObject(key, defaultValue);

    public static void SetColor(string key, Color value) => SetObject(key, value);
    public static Color GetColor(string key, Color defaultValue) => GetObject(key, defaultValue);

    public static void SetColor32(string key, Color32 value) => SetObject(key, value);
    public static Color32 GetColor32(string key, Color32 defaultValue) => GetObject(key, defaultValue);

    public static void SetRect(string key, Rect value) => SetObject(key, value);
    public static Rect GetRect(string key, Rect defaultValue) => GetObject(key, defaultValue);

    // Collections
    public static void SetList<T>(string key, List<T> list) => SetObject(key, list);
    public static List<T> GetList<T>(string key) => GetObject(key, new List<T>());

    public static void SetDictionary<K, V>(string key, Dictionary<K, V> dict) => SetObject(key, dict);
    public static Dictionary<K, V> GetDictionary<K, V>(string key) => GetObject(key, new Dictionary<K, V>());

    // JSON Serialization (Newtonsoft.Json)
    public static void SetObject<T>(string key, T obj) => PlayerPrefs.SetString(key, JsonConvert.SerializeObject(obj));
    public static T GetObject<T>(string key, T defaultValue = default) => PlayerPrefs.HasKey(key) ? JsonConvert.DeserializeObject<T>(PlayerPrefs.GetString(key)) : defaultValue;

    // Deleting Data
    public static void DeleteKey(string key) => PlayerPrefs.DeleteKey(key);
    public static void ClearAll() => PlayerPrefs.DeleteAll();

}