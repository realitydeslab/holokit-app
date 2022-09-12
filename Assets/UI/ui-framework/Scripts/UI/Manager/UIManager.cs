using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  save all UI data and create/detroy UI
/// </summary>
namespace Holoi.HoloKit.App.UI
{
    public class UIManager
    {
        private Dictionary<UIType, GameObject> _dicUI;

        public UIManager()
        {
            _dicUI = new Dictionary<UIType, GameObject>();
        }

        public GameObject CreateUIGO(UIType type)
        {
            var parent = GameObject.Find("Canvas");

            if (!parent)
            {
                Debug.LogError("Canvas not found, please check.");
                return null;
            }

            if (_dicUI.ContainsKey(type))
            {
                _dicUI.Remove(type);
                Debug.Log("contain key");
                //return _dicUI[type];
            }

            Debug.Log("create panel ui go:");
            GameObject ui = GameObject.Instantiate(Resources.Load<GameObject>(type.Path), parent.transform);
            Debug.Log("create panel ui go done!");
            ui.name = type.Name;
            _dicUI.Add(type, ui);
            return ui;
    }

        public void DestroyUI(UIType type)
        {
            if (_dicUI.ContainsKey(type))
            {
                Debug.Log($"delete dic with {type.Name}");
                GameObject.Destroy(_dicUI[type]);
                _dicUI.Remove(type);
            }
        }
    }
}
