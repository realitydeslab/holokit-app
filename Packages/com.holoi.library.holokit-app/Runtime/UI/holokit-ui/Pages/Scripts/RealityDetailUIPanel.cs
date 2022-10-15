using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    [ExecuteInEditMode]
    public class RealityDetailUIPanel : MonoBehaviour
    {
        [SerializeField] Reality _reality;

        public MetaObjectCollectionList availableMetaObjectCollectionList;
        public MetaAvatarCollectionList availableMetaAvatarCollectionList;

        public List<MetaObjectCollection> realityCompatibleMetaObjectCollectionList;
        public List<MetaAvatarCollection> realityCompatibleMetaAvatarCollectionList;

        [Header("Rotate Helper")]
        [SerializeField] ScrollRect _rotateHelperScrollRect;
        [SerializeField] Scrollbar _horizentalScrollBar;
        [SerializeField] Scrollbar _veticalScrollBar;
        [Header("UI Elements")]
        [SerializeField] Transform _content;
        [SerializeField] FlexibleUIText _id;
        [SerializeField] FlexibleUIText _name;
        [SerializeField] FlexibleUIText _author;
        [SerializeField] FlexibleUIText _description;
        [SerializeField] TechTagContainer _technicContainer;
        [SerializeField] FlexibleUIText _version;
        [SerializeField] Transform _videoContainer;

        float _contentInitPositionY;


        private void Awake()
        {
        }

        private void Start()
        {
            if(HoloKitApp.Instance.CurrentReality == null)
            {
                Debug.Log("error: current relity is null");
            }
            else
            {
                _reality = HoloKitApp.Instance.CurrentReality;
            }
            _contentInitPositionY = _content.position.y;

            SetUIInfo();
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                if (PanelManager.Instance.GetActivePanel().UIType.Name == "RealityDetailPanel")
                {
                    UpdateThumbnailPosition();
                }
            }
            else
            {
                UpdateThumbnailPosition();
            }
        }

        public void SetUIInfo()
        {
            _technicContainer.reality = _reality;
            _technicContainer.SetUIInfo();

            _id.text.text = "Reality #" + _reality.id;
            _name.text.text = _reality.name;
            _version.text.text = _reality.version;
            _author.text.text = _reality.author;
            _description.text.text = _reality.description;
            _description.GetComponent<RectTransform>().sizeDelta = new Vector2(
                1086,
                _description.text.preferredHeight);
            //UpdateRealityCompatibleCollections();
            _horizentalScrollBar.value = 0.422f; // default rotation value
        }

        void UpdateThumbnailPosition()
        {
            var deltaY = _content.transform.position.y - _contentInitPositionY;
            var realityThumbnailContainer = FindObjectOfType<RealityThumbnailContainer>();
            realityThumbnailContainer.scrollOffset = new Vector3(0, deltaY, 0);
            realityThumbnailContainer.rotateValue = new Vector2(_horizentalScrollBar.value, _veticalScrollBar.value);
        }

        //void UpdateRealityCompatibleCollections()
        //{
        //    //Debug.Log("UpdateRealityCollections");
        //    var objectTags = _reality.compatibleMetaObjectTags;
        //    var avatarTags = _reality.compatibleMetaAvatarTags;
        //    //Debug.Log($"objectTags: {objectTags.Count}");
        //    //Debug.Log($"avatarTags: {avatarTags.Count}");

        //    realityCompatibleMetaAvatarCollectionList = new List<MetaAvatarCollection>();
        //    realityCompatibleMetaObjectCollectionList = new List<MetaObjectCollection>();

        //    if (availableMetaAvatarCollectionList.list.Count == 0)
        //    {
        //        Debug.Log("there is no available avatar collection");
        //    }
        //    else
        //    {
        //        foreach (var tag in avatarTags)
        //        {
        //            for (int i = 0; i < availableMetaAvatarCollectionList.list.Count; i++)
        //            {
        //                if (availableMetaAvatarCollectionList.list[i].tags.Contains(tag))
        //                {
        //                    realityCompatibleMetaAvatarCollectionList.Add(availableMetaAvatarCollectionList.list[i]);
        //                }
        //                else
        //                {
        //                }
        //            }
        //        }
        //    }

        //    if (availableMetaObjectCollectionList.list.Count == 0)
        //    {
        //        Debug.Log("there is no available object collection");
        //    }
        //    else
        //    {
        //        foreach (var tag in objectTags)
        //        {
        //            for (int i = 0; i < availableMetaObjectCollectionList.list.Count; i++)
        //            {
        //                if (availableMetaObjectCollectionList.list[i].tags.Contains(tag))
        //                {
        //                    realityCompatibleMetaObjectCollectionList.Add(availableMetaObjectCollectionList.list[i]);
        //                }
        //                else
        //                {

        //                }
        //            }
        //        }
        //    }
        //}
    }
}