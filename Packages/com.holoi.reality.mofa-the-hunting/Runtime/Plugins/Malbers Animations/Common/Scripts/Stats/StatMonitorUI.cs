using MalbersAnimations.Scriptables;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MalbersAnimations
{
    /// <summary> Script to Update in a Canvas the changes of the stats</summary>
    [DefaultExecutionOrder(-501)]
    public class StatMonitorUI : MonoBehaviour
    {
        public class StatUI
        {
            public Slider slider;
            public Stat stat;
            public Transform followTransform;
            public UnityAction<float> OnStatValueChange = delegate { };
        }

        [Tooltip("Runtime Set that store all the Stat you want to monitor")]
        [RequiredField] public RuntimeStats Set;
        [Tooltip("Slider used to Represet the Stat on the UI")]
        [RequiredField] public Slider UIPrefab;
        [Tooltip("What stat inside the Stat Manager you want to monitor")]
        public StatID statID;
        [Tooltip("Reference for the Camera")]
        public TransformReference Camera;

        [Tooltip("Find a bone inside the Hierarchy of the Stat Manager")]
        public string FollowTransform = "Head";
        [Tooltip("Use the Normalize value of the Stat")]
        public bool Normalized = true;
        [Tooltip("When the Stat is Empty, Stop Monitoring it")]
        public bool RemoveOnEmpty = true;

        [Tooltip("Offset to Position the Slider UI on the screen")]
        public Vector3 Offset = Vector3.zero;
        [Tooltip("Scale of the Instantiated prefab")]
        public Vector3 Scale = Vector3.one;
        
        private List<StatUI> TrackedStats;
        
        private Camera MainCamera;

        private void Awake()
        {
            TrackedStats = new List<StatUI>();

            Set.Clear();


            if (Camera.Value != null)
            {
                MainCamera = Camera.Value.GetComponent<Camera>();
            }
            else
            {
                MainCamera = MTools.FindMainCamera();
                Camera.Value = MainCamera.transform;
            }
        }


        private void OnEnable()
        {
            Set.OnItemAdded.AddListener(OnAddedStat);
            Set.OnItemRemoved.AddListener(OnRemovedStat);
        }

      

        private void OnDisable()
        {
            Set.OnItemAdded.RemoveListener(OnAddedStat);
            Set.OnItemRemoved.RemoveListener(OnRemovedStat);
        }


        /// <summary>  Track if the Stat change value  </summary>
        private void OnStatValueChanged(float value, StatUI item)
        {
            if (item != null)
            {
                item.slider.value = Normalized ? item.stat.NormalizedValue : item.stat.Value;
            }
        }

        private void OnAddedStat(Stats stats)
        {
            var item = new StatUI();
            item.stat = stats.Stat_Get(statID);

            var child = stats.transform.FindGrandChild(FollowTransform);

            item.followTransform = child != null ? child : stats.transform;
            item.slider = Instantiate(UIPrefab, transform);
            item.slider.transform.localScale = Scale;

            //Track when the Stat changes value
            item.OnStatValueChange = (floatValue) =>
            {
               item.slider.value = Normalized ? item.stat.NormalizedValue : item.stat.Value;

               if (RemoveOnEmpty && item.stat.Value == item.stat.MinValue) RemoveFromGroup(item); //Meaning 
            };


            item.slider.value = Normalized ? item.stat.NormalizedValue : item.stat.Value; //First value.

            item.stat.OnValueChange.AddListener(item.OnStatValueChange);
            TrackedStats.Add(item);
        }


        private void OnRemovedStat(Stats stats)
        {
            var item = TrackedStats.Find(x => x.stat.Owner == stats);

            if (item != null)
            {
                RemoveFromGroup(item);
            }
        }

        private void RemoveFromGroup(StatUI item)
        {
            item.stat.OnValueChange.RemoveListener(item.OnStatValueChange);
            item.OnStatValueChange = null;
            Destroy(item.slider.gameObject);
            TrackedStats.Remove(item);
            Set.Item_Remove(item.stat.Owner);
        }

        private void Update()
        {
            if (MainCamera == null) return;

            foreach (var item in TrackedStats)
            {
                if (item.followTransform)
                {
                    var Pos = MainCamera.WorldToScreenPoint(item.followTransform.position + Offset);
                    item.slider.transform.position = Pos;
                    item.slider.gameObject.SetActive(DoHideOffScreen(Pos));
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