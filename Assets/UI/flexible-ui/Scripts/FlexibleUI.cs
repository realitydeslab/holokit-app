using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    [ExecuteInEditMode]
    public class FlexibleUI : MonoBehaviour
    {
        private static FlexibleUI _instance; // using instance to maintain the state of ui scenes.
        public static FlexibleUI Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new FlexibleUI();
                }
                return _instance;
            }
        }

        public FlexibleUIData SkinData;

        protected virtual void OnSkinUI()
        {

        }

        public virtual void Awake()
        {
            OnSkinUI();
        }

        public virtual void Update()
        {
            if (Application.isEditor)
            {
                OnSkinUI();
            }
        }
    }
}