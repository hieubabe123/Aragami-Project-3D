using UnityEngine;

public class SingletonResources<T> : ScriptableObject where T : SingletonResources<T> {
    static string path = $"{typeof(T)}/{typeof(T)}";

    private static T instance;
    public static T Instance {
        get {
            if (instance == null) {
                instance = Resources.Load<T>(path);
                if (instance == null) {
                    Debug.LogError($"{path} is not exists");
                }
            }

            return instance;
        }
    }

   public bool IsInitallized => instance != null;
}
