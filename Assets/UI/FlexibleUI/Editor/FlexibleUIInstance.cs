using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class FlexibleUIInstance : Editor
{
    [MenuItem("GameObject/HolikitX UI/Text/H1", priority = 0)]
    public static void AddH1()
    {
        Create("H1");
    }
    [MenuItem("GameObject/HolikitX UI/Text/H2", priority = 0)]
    public static void AddH2()
    {
        Create("H2");
    }
    [MenuItem("GameObject/HolikitX UI/Text/Paragraph", priority = 0)]
    public static void AddParagraph()
    {
        Create("Paragraph");
    }
    [MenuItem("GameObject/HolikitX UI/Button/Button", priority =0)]
    public static void AddButton()
    {
        Create("ButtonSample");
    }

    [MenuItem("GameObject/HolikitX UI/Button/ExitButton", priority = 0)]
    public static void AddExitButton()
    {
        Create("ExitButton");
    }
    [MenuItem("GameObject/HolikitX UI/Button/BackButton", priority = 0)]
    public static void AddBackButton()
    {
        Create("BackButton");
    }
    [MenuItem("GameObject/HolikitX UI/Button/RecordButton", priority = 0)]
    public static void AddRecordButton()
    {
        Create("RecordButton");
    }
    [MenuItem("GameObject/HolikitX UI/Button/SpectatorButton", priority = 0)]
    public static void AddSpectatorButton()
    {
        Create("SpectatorButton");
    }
    [MenuItem("GameObject/HolikitX UI/Button/StarButton", priority = 0)]
    public static void AddStarButton()
    {
        Create("StarButton");
    }
    [MenuItem("GameObject/HolikitX UI/Button/PermissionButton", priority = 0)]
    public static void AddPermissionButton()
    {
        Create("PermissionButton");
    }
    [MenuItem("GameObject/HolikitX UI/Component/Setting", priority = 0)]
    public static void AddSetting()
    {
        Create("Setting");
    }
    [MenuItem("GameObject/HolikitX UI/Logo/Logo", priority = 0)]
    public static void AddLogo()
    {
        Create("Logo");
    }



    static GameObject _clickedObject;

    static GameObject Create(string objectName)
    {
        GameObject instance = Instantiate(Resources.Load<GameObject>(objectName));
        instance.name = objectName;

        _clickedObject = UnityEditor.Selection.activeObject as GameObject;
        if (_clickedObject != null)
        {
            instance.transform.SetParent(_clickedObject.transform);
        }

        return instance;
    }
}
