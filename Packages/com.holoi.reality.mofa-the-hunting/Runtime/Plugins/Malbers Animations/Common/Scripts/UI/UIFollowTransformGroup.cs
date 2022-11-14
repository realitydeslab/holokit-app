using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MalbersAnimations.Scriptables;

namespace MalbersAnimations.UI
{
    /// <summary>
    /// Use a list of Gameobjects to Position UI on Top of them
    /// </summary>
    [DefaultExecutionOrder(-500)]
    public class UIFollowTransformGroup : MonoBehaviour
    {
        public class UIFollowItem
        {
            public GameObject worldObject;
            public GameObject UI;

            public UIFollowItem(GameObject world, GameObject ui)
            {
                worldObject = world;
                UI = ui;
            }
        }

        [Tooltip("Reference for the Camera")]
        public TransformReference Camera;

        [CreateScriptableAsset] public RuntimeGameObjects Set;
        [RequiredField] public GameObject ItemUI;
       
       

        private Camera cam;

        private List<UIFollowItem> items;
        public bool HideOffScreen = true;

        [Tooltip("Offset position for the tracked gameobject")]
        public Vector3 Offset = Vector3.zero;
        [Tooltip("Scale of the Instantiated prefab")]
        public Vector3 Scale = Vector3.one;


        private void Awake()
        {
            items = new List<UIFollowItem>();

            if (!Set) { enabled = false; Debug.LogWarning($"{name} Does not have a runtime set to follow", this);  return; }

            Set.Clear();

            if (Camera.Value != null)
            {
                cam = Camera.Value.GetComponent<Camera>();
            }
            else
            {
                cam = MTools.FindMainCamera();
                Camera.Value = cam.transform;
            }
        }

        private void OnEnable()
        {
            if (ItemUI == null) { enabled = false; return; }

            if (Set != null)
            {
                Set.OnItemAdded.AddListener(OnItemAdded);
                Set.OnItemRemoved.AddListener(OnItemRemoved);
                Set.OnSetEmpty.AddListener(OnSetEmpty);
            }

            cam = MTools.FindMainCamera();
            items = new List<UIFollowItem>();
        }

        private void OnDisable()
        {
            if (Set != null)
            {
                Set.OnItemAdded.RemoveListener(OnItemAdded);
                Set.OnItemRemoved.RemoveListener(OnItemRemoved);
                Set.OnSetEmpty.RemoveListener(OnSetEmpty);
            }
        }

        public void ChangeObjectSet(RuntimeGameObjects newObjects)
        {
            //unlink the current sets
        }


        private void OnSetEmpty()
        {
            //???
        }

        private void OnItemRemoved(GameObject removedGo)
        {
            var index = items.Find(x => x.worldObject == removedGo);

            if (index != null)
            {
                Destroy(index.UI);
                items.Remove(index);
            }
        }

        private void OnItemAdded(GameObject worldObject)
        {
            var newUIItem = Instantiate(ItemUI);

            newUIItem.name = "Icon - " + worldObject.name;
            newUIItem.SetActive(true);
            newUIItem.transform.SetParent(this.transform, false);

            items.Add(new UIFollowItem(worldObject, newUIItem));

            newUIItem.transform.localScale = Scale;
        }

        // Update is called once per frame
        void Update()
        {
            if (cam && items.Count > 0)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var pos  =  cam.WorldToScreenPoint(items[i].worldObject.transform.position + Offset);

                    items[i].UI.transform.position = pos;

                    if (HideOffScreen)
                    {
                        items[i].UI.gameObject.SetActive(DoHideOffScreen(pos));
                    }
                    else
                    {
                        if (pos.z < 0)
                        {
                            pos.y = pos.y > Screen.height / 2 ? 0 : Screen.height;
                        }
                        items[i].UI.transform.position = new Vector3(
                            Mathf.Clamp(pos.x, 0, Screen.width),
                            Mathf.Clamp(pos.y, 0, Screen.height),
                           0);
                    }
                }
            }
        }

        private bool DoHideOffScreen(Vector3 position)
        {
            if (position.x < 0 || position.x > Screen.width) return false;
            if (position.y < 0 || position.y > Screen.height) return false;
            if (position.z < 0) return false;

            return true;
        }
 
    } 
}