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
