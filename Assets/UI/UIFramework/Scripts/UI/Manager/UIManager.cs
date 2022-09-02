using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  save all UI data and create/detroy UI
/// </summary>
public class UIManager
{
    private Dictionary<UIType, GameObject> _dicUI;

    public UIManager()
    {
        _dicUI = new Dictionary<UIType, GameObject>();
    }

    public GameObject GetUIGO(UIType type)
    {
        var parent = GameObject.Find("Canvas");

        if (!parent)
        {
            Debug.LogError("Canvas not found, please check.");
            return null;
        }
        if (_dicUI.ContainsKey(type))
            return _dicUI[type];

        GameObject ui = GameObject.Instantiate(Resources.Load<GameObject>(type.Path), parent.transform);
        ui.name = type.Name;
        _dicUI.Add(type, ui);
        return ui;
    }

    public void DestroyUI(UIType type)
    {
        if (_dicUI.ContainsKey(type))
        {
            GameObject.Destroy(_dicUI[type]);
            _dicUI.Remove(type);
        }
    }
}
