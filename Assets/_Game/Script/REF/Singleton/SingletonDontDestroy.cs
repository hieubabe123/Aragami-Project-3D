using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonDontDestroy<T> : SingletonMonoBehaviour<T> where T : MonoBehaviour
{
    private static bool isInitialized = false;
    private void Awake() {
        if (isInitialized) {
            Destroy(gameObject); 
            return;
        }
        isInitialized = true;
        DontDestroyOnLoad(this.gameObject);
    }
}
