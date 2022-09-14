using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    public class TechTagContainer : MonoBehaviour
    {
        public Holoi.AssetFoundation.Reality reality;
        [SerializeField] GameObject _techButtonPrefab;

        void Start()
        {
            foreach (var tag in reality.realityTags)
            {

            }

        }

        void CreateTechButton(RealityTag tag)
        {
            if(tag.tagName == "Hand Tracking")
            {
                _techButtonPrefab.GetComponent<FlexibleUITechButton>().type = 0;
                var go = Instantiate(_techButtonPrefab, transform);
            }
        }
    }
}
