using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class UITool
    {
        GameObject _activePanelGO;

        public GameObject ActivePanelGO
        {
            get { return _activePanelGO; }
            set { _activePanelGO = value; }
        }

        public UITool(GameObject panel)
        {
            _activePanelGO = panel;
        }

        public T GetOrAddComponent<T>() where T : Component
        {
            if(_activePanelGO == null)
            {
                Debug.Log("not found activePanelGO");
                return null;
            }
            else
            {
                if (_activePanelGO.GetComponent<T>() == null)
                {
                    //Debug.Log("(_activePanel.GetComponent<T>() == null");
                    _activePanelGO.AddComponent<T>();
                }
                else
                {
                    //Debug.Log("(_activePanel.GetComponent<T>() !-=null");
                }
                return _activePanelGO.GetComponent<T>();
            }

            
        }

        public GameObject FindGameObjectInChindren(string name)
        {
            Transform[] children;

            if (_activePanelGO == null)
            {
                Debug.Log("not found activepanelGO");
                children = new Transform[0];
            }
            else
            {
                children = _activePanelGO.GetComponentsInChildren<Transform>();
            }
            

            foreach (var child in children)
            {
                if (child.name == name)
                {
                    return child.gameObject;
                }
            }

            Debug.LogError($"{_activePanelGO.name} does not exist a child named: { name }");
            return null;
        }

        public List<GameObject> FindGameObjectsInChildren(string name)
        {
            List<GameObject> childrenList = new List<GameObject>();
            Transform[] children = _activePanelGO.GetComponentsInChildren<Transform>();

            foreach (var child in children)
            {
                if (child.name == name)
                {
                    childrenList.Add(child.gameObject);
                }
            }
            return childrenList;
        }

        public T GetOrAddComponentInChildren<T>(string name) where T : Component
        {
            var child = FindGameObjectInChindren(name);
            if (child != null)
            {
                if (child.GetComponent<T>() == null)
                    child.AddComponent<T>();

                return child.GetComponent<T>();
            }
            else
            {
                Debug.LogError("GetOrAddComponentInChildren with a result: null found.");
            }
            return null;
        }
    }
}