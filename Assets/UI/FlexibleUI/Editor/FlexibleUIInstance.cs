using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class FlexibleUIInstance : Editor
{
    [MenuItem("GameObject/HolikitX UI/Button", priority =0)]
    public static void AddButton()
    {
        Create("ButtonSample");
    }

    [MenuItem("GameObject/HolikitX UI/ExitButton", priority = 0)]
    public static void AddExitButton()
    {
        Create("ExitButton");
    }
    [MenuItem("GameObject/HolikitX UI/BackButton", priority = 0)]
    public static void AddBackButton()
    {
        Create("BackButton");
    }
    [MenuItem("GameObject/HolikitX UI/RecordButton", priority = 0)]
    public static void AddRecordButton()
    {
        Create("RecordButton");
    }
    [MenuItem("GameObject/HolikitX UI/SpectatorButton", priority = 0)]
    public static void AddSpectatorButton()
    {
        Create("SpectatorButton");
    }
    [MenuItem("GameObject/HolikitX UI/StarButton", priority = 0)]
    public static void AddStarButton()
    {
        Create("StarButton");
    }
    [MenuItem("GameObject/HolikitX UI/Setting", priority = 0)]
    public static void AddSetting()
    {
        Create("Setting");
    }
    [MenuItem("GameObject/HolikitX UI/Logo", priority = 0)]
    public static void AddLogo()
    {
        Create("Logo");
    }
    [MenuItem("GameObject/HolikitX UI/PermissionButton", priority = 0)]
    public static void AddPermissionButton()
    {
        Create("PermissionButton");
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
