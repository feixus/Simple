using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UI;

[CustomEditor(typeof(LuaBehavior))]
public class BehaviorEditor : Editor
{
    internal const float k_SingleLineHeight = 16f;

    private LuaBehavior Behavior { get { return (target as LuaBehavior); } }

    private SerializedObject m_SerializedObject;
    private ReorderableList m_ReorderableList;

    void OnEnable()
    {
        m_SerializedObject = new SerializedObject(target);
        m_ReorderableList = CreateReorderList(m_SerializedObject);
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Fill", GUILayout.Height(25)))
        {
            Fill();
        }

        GUILayout.Space(5);
        if (GUILayout.Button("Edit", GUILayout.Height(25)))
        {
            Edit();
        }
        GUILayout.EndHorizontal();

        m_SerializedObject.Update();
        EditorGUI.BeginChangeCheck();
         
        EditorGUILayout.PropertyField(m_SerializedObject.FindProperty("m_LuaFile"));
        EditorGUILayout.PropertyField(m_SerializedObject.FindProperty("m_UpdateOrder"));

        GUILayout.Space(10);
        if (m_ReorderableList != null)
            m_ReorderableList.DoLayoutList();

        if (EditorGUI.EndChangeCheck())
        {
            m_SerializedObject.ApplyModifiedProperties();
        }
    }

    private ReorderableList CreateReorderList(SerializedObject serializeObj)
    {
        var bindVariables = serializeObj.FindProperty("m_BindVariables");
        var reorderableList = new ReorderableList(serializeObj, bindVariables, true, true, true, true)
        {
            drawHeaderCallback = (Rect rect) =>
            {
                GUI.Label(rect, "Bind Variables");
            },
            drawElementCallback = (Rect rect, int index, bool isactive, bool isfocused) =>
            {
                var bindVariable = bindVariables.GetArrayElementAtIndex(index);
                var valueProperty = bindVariable.FindPropertyRelative("Value");
                var nameProperty = bindVariable.FindPropertyRelative("Name");

                valueProperty.objectReferenceValue = EditorGUI.ObjectField(new Rect(rect.xMin, rect.yMin, rect.width / 2.0f, k_SingleLineHeight), "", valueProperty.objectReferenceValue, typeof(Object), true);
                nameProperty.stringValue = EditorGUI.TextField(new Rect(rect.xMin + rect.width / 2.0f, rect.yMin, rect.width / 2.0f, k_SingleLineHeight), nameProperty.stringValue);

                SetDefaultVariableName(valueProperty.objectReferenceValue, nameProperty);
            },
            onAddCallback = (ReorderableList list) =>
            {
                bindVariables.InsertArrayElementAtIndex(bindVariables.arraySize);
                
                var bindVariable = bindVariables.GetArrayElementAtIndex(bindVariables.arraySize - 1);
                bindVariable.FindPropertyRelative("Value").objectReferenceValue = null;
                bindVariable.FindPropertyRelative("Name").stringValue = "";
            },
            onReorderCallback = (ReorderableList list) =>
            {
                SaveTile();
            }
        };

        return reorderableList;
    }

    private void SaveTile()
    {
        EditorUtility.SetDirty(target);
        SceneView.RepaintAll();
    }

    private void Fill()
    {
        m_SerializedObject.Update();

        Button[] btns = Behavior.transform.GetComponentsInChildren<Button>();
        Text[] txts = Behavior.transform.GetComponentsInChildren<Text>();

        InsertProperty(btns);
        InsertProperty(txts);

        m_SerializedObject.ApplyModifiedProperties();
    }

    private void InsertProperty(UnityEngine.Object[] objs)
    {
        var bindVariables = m_SerializedObject.FindProperty("m_BindVariables");

        for (int n = 0, max = objs.Length; n < max; n++)
        {
            Object obj = objs[n];
            bool needInsert = true;

            for (int i = 0, cMax = bindVariables.arraySize; i < cMax; i++)
            {
                var variable = bindVariables.GetArrayElementAtIndex(i);
                if (variable.FindPropertyRelative("Value").objectReferenceValue == obj)
                {
                    needInsert = false;
                    break;
                }
            }

            if (needInsert)
            {
                bindVariables.InsertArrayElementAtIndex(bindVariables.arraySize);

                var newVariable = bindVariables.GetArrayElementAtIndex(bindVariables.arraySize - 1);
                var valueProperty = newVariable.FindPropertyRelative("Value");
                var nameProperty = newVariable.FindPropertyRelative("Name");

                valueProperty.objectReferenceValue = obj;
                nameProperty.stringValue = "";

                SetDefaultVariableName(obj, nameProperty);
            } 
        }   
    }

    private void Edit()
    {
        MyWindow myWindow = MyWindow.ShowWindow();
        myWindow.m_ReorderableList = m_ReorderableList;
        myWindow.m_SerializedObject = m_SerializedObject;
    }

    private void SetDefaultVariableName(UnityEngine.Object obj, SerializedProperty nameProperty)
    {
        if (string.IsNullOrEmpty(nameProperty.stringValue) && obj != null)
        {
            nameProperty.stringValue = GetVarName(obj);
        }
    }

    private string GetVarName(Object obj)
    {
        string prefix = "";
        if (obj is Button)
        {
            prefix = "btn_";
        }
        else if(obj is Text)
        {
            prefix = "txt_";
        }
        else if (obj is Image)
        {
            prefix = "img_";
        }
        else if (obj is Slider)
        {
            prefix = "slider_";
        }
        else if (obj is ScrollRect)
        {
            prefix = "scroll_";
        }

        return string.Format("{0}{1}", prefix, obj.name);
    }

    public class MyWindow : EditorWindow
    {
        public SerializedObject m_SerializedObject;
        public ReorderableList m_ReorderableList;

        public static MyWindow ShowWindow()
        {
           return EditorWindow.GetWindow<MyWindow>(true, "Lua Behavior", true);
        }

        void OnGUI()
        {
            if (m_SerializedObject != null)
                m_SerializedObject.Update();

            EditorGUI.BeginChangeCheck();
            if (m_ReorderableList != null)
            {
                m_ReorderableList.DoLayoutList();
            }
              
            if (m_SerializedObject != null && EditorGUI.EndChangeCheck())
                m_SerializedObject.ApplyModifiedProperties();
        }
    }
}
