using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.AssetFoundation;

namespace Holoi.HoloKit.App.UI
{
    public class TechTagContainer : MonoBehaviour
    {
        Holoi.AssetFoundation.Reality _reality;
        [SerializeField] GameObject _techButtonPrefab;

        void Start()
        {
            foreach (var tag in _reality.realityTags)
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
