using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class FlexibleUIInstance : Editor
{
    [MenuItem("GameObject/Flexible UI/Button", priority =0)]
    public static void AddButton()
    {
        Create("ButtonSample");
    }

    [MenuItem("GameObject/Flexible UI/ExitButton", priority = 0)]
    public static void AddExitButton()
    {
        Create("ExitButton");
    }
    [MenuItem("GameObject/Flexible UI/BackButton", priority = 0)]
    public static void AddBackButton()
    {
        Create("BackButton");
    }
    [MenuItem("GameObject/Flexible UI/RecordButton", priority = 0)]
    public static void AddRecordButton()
    {
        Create("RecordButton");
    }
    [MenuItem("GameObject/Flexible UI/SpectatorButton", priority = 0)]
    public static void AddSpectatorButton()
    {
        Create("SpectatorButton");
    }
    [MenuItem("GameObject/Flexible UI/StarButton", priority = 0)]
    public static void AddStarButton()
    {
        Create("StarButton");
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
