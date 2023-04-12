using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: Component
{
    private static bool applicationIsQuitting = false;
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
    protected virtual void Awake()
    {
        Application.quitting += () => applicationIsQuitting = true;
        _instance = this as T;
    }
}
