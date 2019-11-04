using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager s_Instance;
    public static ResourceManager Instance 
    {
        get {return s_Instance;}
    }

    void Awake()
    {
        s_Instance = this; 
    }

    public static Object LoadPrefab(string path)
    {
        Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
        if (obj == null)
            return null;

        return Instantiate(obj);
    }
    
}
