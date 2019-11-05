using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUp : MonoBehaviour
{
    void Start()
    {
        var sceneRoot = ResourceManager.LoadPrefab("assets/prefabs/ui/sceneroot.prefab");
        sceneRoot.name = "SceneRoot";
        DontDestroyOnLoad(sceneRoot);

        GameObject newObj = new GameObject("ResourceManager");
        newObj.AddComponent<ResourceManager>();
        DontDestroyOnLoad(newObj);

        // load LuaManager
        newObj = new GameObject("LuaManager");
        newObj.AddComponent<LuaManager>();
        DontDestroyOnLoad(newObj);

        StartCoroutine(DelayLoad());
    }

    IEnumerator DelayLoad()
    {
        yield return new WaitForSeconds(1.0f);

        LuaManager.Instance.DoFile("init", "require \"init\"", null);
    }

}
