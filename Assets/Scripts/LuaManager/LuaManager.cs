using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using XLua;

using UnityEditor;

namespace XLua
{
    public partial class ObjectTranslator
    {
        public void ClearDelegateBriages()
        {
            delegate_bridges.Clear();
        }
    }

    public class LuaEnvEx : LuaEnv
    {
        public void ForceDispose()
        {
            FullGc();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

            translator.ClearDelegateBriages();
            Dispose(true);

            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }
    }
}


[DefaultExecutionOrder(-1000)]
public class LuaManager : MonoBehaviour
{
    private static LuaManager s_Instance = null;
    public static LuaManager Instance
    {
        get { return s_Instance; }
    }

    private LuaEnvEx m_LuaEnv;
    public LuaTable Global { get {return m_LuaEnv == null ? null : m_LuaEnv.Global;}}

    void Awake()
    {
        s_Instance = this;
    }

    void Start()
    {
        m_LuaEnv = new LuaEnvEx();

        m_LuaEnv.AddLoader(CustomLoader);
    }

    void Update()
    {
        if (m_LuaEnv != null)
            m_LuaEnv.Tick();
    }

    //为了让LuaManager的OnDestroy最后执行，设置了DefaultExecutionOrder(-10000)
    //但若LuaBehavior与LuaManager不同属于DontDestroyOnLoad时会无视此设置(可能是执行顺序的无序性吧？？？)
    void OnDestroy()
    {
        Debug.Log("LuaManager  OnDestroy");
        if (m_LuaEnv != null)
        {
            m_LuaEnv.ForceDispose();
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
