using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoi.AssetFoundation;


namespace Holoi.HoloKit.App.UI
{
    [ExecuteInEditMode]
    public class TechTagContainer : MonoBehaviour
    {
        public Holoi.AssetFoundation.Reality reality;
        [SerializeField] GameObject _techButtonPrefab;

        private void Awake()
        {
            ClearPreviousGenerativeContent();
            if (reality != null)
            {
                CreateTechTags(reality);
            }
            else
            {
                Debug.LogError("reality of TechTagContainer is null");
            }
        }

        void ClearPreviousGenerativeContent()
        {
            var tempList = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                tempList.Add(transform.GetChild(i));
            }
            foreach (var child in tempList)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(child.gameObject);
                }
                else
                {
                    Destroy(child.gameObject);
                }
            }
        }

        void CreateTechTags(Holoi.AssetFoundation.Reality realitySample)
        {
            foreach (var tag in realitySample.realityTags)
            {
                CreateTechTag(tag);
            }
        }

        void CreateTechTag(RealityTag tag)
        {
            _techButtonPrefab.GetComponent<FlexibleUITechButton>().tag = tag;
            var go = Instantiate(_techButtonPrefab, transform);
        }
    }
}
