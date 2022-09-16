using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    [ExecuteInEditMode]
    public class FlexibleUI : MonoBehaviour
    {

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