using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonBehaviourResources<T> : MonoBehaviour where T : MonoBehaviour {
    private static T _instance;

    private static bool _destroying;

    //
    // Summary:
    //     Place your prefab in Resources folder: "Scriptable/T/T", T is the name of class
    protected static string ResourcePath => $"{typeof(T).Name}/{typeof(T).Name}";

    //
    // Summary:
    //     "Instance" = Instantiate from Resources folder when be called at runtime.
    //     Place your prefab in Resources: "Scriptable/T/T", T is the name of class
    public static T Instance {
        get {
            if ((Object)_instance != (Object)null || _destroying) {
                return _instance;
            }

            _instance = Object.FindObjectOfType<T>();
            if ((Object)_instance == (Object)null) {
                GameObject gameObject = Resources.Load<GameObject>(ResourcePath);
                if (gameObject != null) {
                    _instance = Object.Instantiate(gameObject).GetComponent<T>();
                }
                else {
                    Debug.LogError("[" + typeof(T).Name + "] Wrong resources path: " + ResourcePath);
                }
            }

            return _instance;
        }
    }

    //
    // Summary:
    //     Use for checking whether Instance is null or not.
    //     Use "if (Initialized)" instead of "if (Instance != null)"
    public static bool Initialized => (Object)_instance != (Object)null;

    protected virtual bool DontDestroy => false;

    protected abstract void OnAwake();

    //
    // Summary:
    //     DO NOT OVERRIDE THIS METHOD. USE "OnAwake" instead
    private void Awake() {
        if ((Object)_instance == (Object)null) {
            _instance = this as T;
        }
        else if (_instance.GetInstanceID() != GetInstanceID()) {
            Object.Destroy(base.gameObject);
            return;
        }

        if (DontDestroy) {
            Object.DontDestroyOnLoad(base.gameObject);
        }

        OnAwake();
    }

    //
    // Summary:
    //     Call 'T.Instance.Preload()' at the first application script to preload the service.
    public virtual void Preload() {
    }

    //
    // Summary:
    //     If you want to override this method, remember to call this base.
    protected virtual void OnDestroy() {
        _destroying = true;
        if ((Object)_instance != (Object)null && _instance.GetInstanceID() == GetInstanceID()) {
            _instance = null;
        }
    }
}


public abstract class SingletonBehaviourResourcesDontDestroy<T> : SingletonBehaviourResources<T> where T : MonoBehaviour {
    protected override bool DontDestroy => true;
}

//
// Summary:
//     "Instance" = Instantiate from Resources folder when be called at runtime.
//     Place your prefab in Resources: "T/T", T is the name of class
public abstract class SingletonResourcesScriptable<T> : ScriptableObject where T : ScriptableObject {
    //
    // Summary:
    //     This is local variable. Use 'Instance' from outsite.
    protected static T __instance;

    //
    // Summary:
    //     If you want to check null, use this property instead of calling to "Instance"
    //     because "Instance" can auto create an emty gameobject, so "Instance" can never
    //     null
    public static bool Initialized => (Object)__instance != (Object)null;

    //
    // Summary:
    //     Place your prefab in Resources folder: "Scriptable/T/T", T is the name of class
    protected static string ResourcePath => $"{typeof(T).Name}/{typeof(T).Name}";

    //
    // Summary:
    //     "Instance" = Instantiate from Resources folder when be called at runtime.
    //     Place your prefab in Resources: "Scriptable/T/T", T is the name of class
    public static T Instance {
        get {
            if ((Object)__instance != (Object)null) {
                return __instance;
            }

            T val = Resources.Load<T>(ResourcePath);
            if ((Object)val != (Object)null) {
                __instance = Object.Instantiate(val);
                (__instance as SingletonResourcesScriptable<T>).Initialize();
                Debug.Log("[" + typeof(T).Name + "] Initialized.");
            }
            else {
                Debug.LogError("[" + typeof(T).Name + "] Wrong resources path: " + ResourcePath + "!");
            }

            return __instance;
        }
    }

    //
    // Summary:
    //     This function will be call automaticaly only one times (on the first call of
    //     "Instance")
    //     Put your custom initialize here. No need to call "base.Initialize"
    protected abstract void Initialize();

    //
    // Summary:
    //     This method is empty function, just use to prepare initialize the "Instance"
    //     to improve Ram/ CPU (to preload or decompress all asset inside)
    //     Call this at the first application script (exp: BasePreload.cs)
    //     Be carefully if you override this and put your custom initialzation here, because
    //     this function can be call many times on any where, so the initialization inside
    //     will be init many times too
    public virtual void Preload() {
    }
}