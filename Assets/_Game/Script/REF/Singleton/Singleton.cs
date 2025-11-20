using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> 
{
    protected static T instance;
    protected static T Instance {
        get { 
            return instance; 
        
        }
    }

    public bool IsInitallized => instance != null;
}
