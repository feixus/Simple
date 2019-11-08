using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[Serializable]
public class BindVariable
{
    public string Name;
    public UnityEngine.Object Value;

    public BindVariable(string _name, UnityEngine.Object _value)
    {
        Name = _name;
        Value = _value;
    }
}

public class LuaBehavior : MonoBehaviour
{
    public TextAsset m_LuaFile;
    public int m_UpdateOrder;
    public List<BindVariable> m_BindVariables = new List<BindVariable>();

    public LuaTable m_ScriptTable { get; private set; }

    private Action lua_Awake;
    private Action lua_OnEnable;
    private Action lua_Start;
    private Action lua_Update;
    private Action lua_LateUpdate;
    private Action lua_OnDisable;
    private Action lua_OnDestroy;

    void Awake()
    {
        if (m_LuaFile != null && !string.IsNullOrEmpty(m_LuaFile.text))
        {
            m_ScriptTable = LuaManager.Instance.NewTable();

            LuaManager.Instance.DoFile(m_LuaFile.name, m_LuaFile.text, m_ScriptTable);

            // m_ScriptTable.Set("this", this);
            m_ScriptTable.Set("self", this);
            foreach (var item in m_BindVariables)
            {
                m_ScriptTable.Set(item.Name, item.Value);
            }

            m_ScriptTable.Get("Awake", out lua_Awake);
            m_ScriptTable.Get("OnEnable", out lua_OnEnable);
            m_ScriptTable.Get("Start", out lua_Start);
            m_ScriptTable.Get("Update", out lua_Update);
            m_ScriptTable.Get("LateUpdate", out lua_LateUpdate);
            m_ScriptTable.Get("OnDisable", out lua_OnDisable);
            m_ScriptTable.Get("OnDestroy", out lua_OnDestroy);
        }

        lua_Awake?.Invoke();
    }

    void OnEnable()
    {
        lua_OnEnable?.Invoke();
    }

    void Start()
    {
        lua_Start?.Invoke();
    }

    void Update()
    {
        lua_Update?.Invoke();
    }

    void LateUpdate()
    {
        lua_LateUpdate?.Invoke();
    }

    void OnDisable()
    {
        lua_OnDisable?.Invoke();
    }

    void OnDestroy()
    {
        lua_OnDestroy?.Invoke();
        if (!LuaManager.Instance.IsLuaEnvNull())
        {
            if (m_ScriptTable != null)
                m_ScriptTable.Dispose();
            m_ScriptTable = null;
        }
    }
}
