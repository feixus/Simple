using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using XLua;

using UnityEditor;

namespace XLua
{
    public partial class LuaEnv
    {
        public void Clear()
        {
            translator.ClearDelegateBriages();
        }
    }

    public partial class ObjectTranslator
    {
        public void ClearDelegateBriages()
        {
            delegate_bridges.Clear();
        }
    }
}



public class LuaManager : MonoBehaviour
{
    private static LuaManager s_Instance = null;
    public static LuaManager Instance
    {
        get { return s_Instance; }
    }

    private LuaEnv m_LuaEnv;
    public LuaTable Global { get {return m_LuaEnv == null ? null : m_LuaEnv.Global;}}

    void Awake()
    {
        s_Instance = this;
    }

    void Start()
    {
        m_LuaEnv = new LuaEnv();

        m_LuaEnv.AddLoader(CustomLoader);
    }

    void Update()
    {
        if (m_LuaEnv != null)
            m_LuaEnv.Tick();
    }

    void OnDestroy()
    {
        if (m_LuaEnv != null)
        {
            m_LuaEnv.Clear();
            m_LuaEnv.Dispose();
        }
            
        m_LuaEnv = null;
    }

    private byte[] CustomLoader(ref string fileName)
    {
        fileName = fileName.Replace(".", "/") + ".txt";
        string luaPath = Application.dataPath + "/Scripts/Lua/" + fileName;
        string luaContent = File.ReadAllText(luaPath);
        byte[] block = Encoding.UTF8.GetBytes(luaContent);

        return block;
    }

    public LuaTable NewTable()
    {
        LuaTable table = m_LuaEnv.NewTable();
        LuaTable meta = m_LuaEnv.NewTable();

        meta.Set("__index", Global);
        table.SetMetaTable(meta);

        meta.Dispose();
        meta = null;

        return table;
    }

    public void DoFile(string fileName, string luaFile, LuaTable table)
    {
        m_LuaEnv.DoString(luaFile, fileName, table);
    }

}
