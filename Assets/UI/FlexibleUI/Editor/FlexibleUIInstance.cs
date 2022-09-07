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
    [MenuItem("GameObject/HolikitX UI/Text/H3", priority = 0)]
    public static void AddH3()
    {
        Create("H3");
    }
    [MenuItem("GameObject/HolikitX UI/Text/Body1", priority = 0)]
    public static void AddBody1()
    {
        Create("Body1");
    }
    [MenuItem("GameObject/HolikitX UI/Text/Body2", priority = 0)]
    public static void AddBody2()
    {
        Create("Body2");
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
    [MenuItem("GameObject/HolikitX UI/Button/StAR Tools", priority = 0)]
    public static void AddStARTools()
    {
        Create("StARTools");
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
    [MenuItem("GameObject/HolikitX UI/Media/MediaList", priority = 0)]
    public static void AddMedia()
    {
        Create("MediaList");
    }
    [MenuItem("GameObject/HolikitX UI/Button/PlayButton", priority = 0)]
    public static void AddPlayButton()
    {
        Create("PlayButton");
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
