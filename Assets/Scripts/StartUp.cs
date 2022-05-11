using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUp : MonoBehaviour
{
    public delegate void NotifyPlayingGame();
    public event NotifyPlayingGame NotifyPlayingGameEvent;

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

        NotifyPlayingGameEvent?.Invoke();

        // XLua.LuaDLL.Lua.xlua_getglobal(LuaManager.Instance.LuaEnv.L, "myname");
        // XLua.LuaDLL.Lua.lua_pushstring(LuaManager.Instance.LuaEnv.L, "name");
        // XLua.LuaDLL.Lua.xlua_pgettable(LuaManager.Instance.LuaEnv.L, -2);
        // string ss = XLua.LuaDLL.Lua.lua_tostring(LuaManager.Instance.LuaEnv.L, -1);

        // int top = XLua.LuaDLL.Lua.lua_gettop(LuaManager.Instance.LuaEnv.L);
        // Debug.Log("c# call lua table value = " + ss);
        // Debug.Log("stack top = " + top);

        // XLua.LuaDLL.Lua.lua_pop(LuaManager.Instance.LuaEnv.L, 1);
        // XLua.LuaDLL.Lua.lua_pushstring(LuaManager.Instance.LuaEnv.L, "age");
        // XLua.LuaDLL.Lua.xlua_pgettable(LuaManager.Instance.LuaEnv.L, -2);
        // double age = XLua.LuaDLL.Lua.lua_tonumber(LuaManager.Instance.LuaEnv.L, -1);
        // Debug.Log("age = " + age);

        // XLua.LuaDLL.Lua.lua_createtable(LuaManager.Instance.LuaEnv.L, 3, 0);
        // XLua.LuaDLL.Lua.lua_pushstring(LuaManager.Instance.LuaEnv.L, "2, 0");
        // XLua.LuaDLL.Lua.lua_pushnumber(LuaManager.Instance.LuaEnv.L, 99);
        // XLua.LuaDLL.Lua.lua_rawset(LuaManager.Instance.LuaEnv.L, -3);
        
        // XLua.LuaDLL.Lua.lua_pushstring(LuaManager.Instance.LuaEnv.L, "bb");
        // XLua.LuaDLL.Lua.lua_pushnumber(LuaManager.Instance.LuaEnv.L, 12);
        // XLua.LuaDLL.Lua.lua_rawset(LuaManager.Instance.LuaEnv.L, -3);

        // XLua.LuaDLL.Lua.xlua_setglobal(LuaManager.Instance.LuaEnv.L, "args");

        // LuaManager.Instance.DoFile("test", "require \"test\"", null);
    }

}
