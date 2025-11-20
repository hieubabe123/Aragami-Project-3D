using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static bool isApplicationQuitting = false;
    public static T Instance
    {
        get
        {
            if (isApplicationQuitting)
            {
                return null;
            }
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject gameObject = new GameObject("Auto generated" + typeof(T));
                    instance = gameObject.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    protected virtual void OnApplicationQuit()
    {
        isApplicationQuitting = true;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        OnAwake();
    }
    protected virtual void OnAwake()
    {

    }
}
