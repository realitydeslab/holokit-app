using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    [ExecuteInEditMode]
    public class RealityDetailUIPanel : MonoBehaviour
    {
        public AssetFoundation.Reality reality;
        public MetaObjectCollectionList availableMetaObjectCollectionList;
        public MetaAvatarCollectionList availableMetaAvatarCollectionList;

        public List<MetaObjectCollection> realityMetaObjectCollectionList;
        public List<MetaAvatarCollection> realityMetaAvatarCollectionList;

        [Header("Rotate Helper")]
        [SerializeField] ScrollRect _rotateHelperScrollRect;
        [SerializeField] Scrollbar _rotateHelperScrollBar;
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
            _contentInitPositionY = _content.position.y;
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
            Debug.Log("SetUIInfo");
            _technicContainer.reality = reality;
            _technicContainer.SetUIInfo();

            _id.text.text = "Reality #" + reality.realityId;
            _name.text.text = reality.name;
            _version.text.text = reality.version;
            _author.text.text = reality.author;
            _description.text.text = reality.description;
            _description.GetComponent<RectTransform>().sizeDelta = new Vector2(
                1002,
                _description.text.preferredHeight);
            UpdateRealityCollections(); 
        }

        void UpdateThumbnailPosition()
        {
            var deltaY = _content.transform.position.y - _contentInitPositionY;
            var realityThumbnailContainer = FindObjectOfType<RealityThumbnailContainer>();
            realityThumbnailContainer.scrollOffset = new Vector3(0, deltaY, 0);
            realityThumbnailContainer.rotateValue = _rotateHelperScrollBar.value;
        }

        void UpdateRealityCollections()
        {
            Debug.Log("UpdateRealityCollections");
            var objectTags = reality.compatibleMetaObjectTags;
            var avatarTags = reality.compatibleMetaAvatarTags;
            Debug.Log($"objectTags: {objectTags.Count}");
            Debug.Log($"avatarTags: {avatarTags.Count}");

            availableMetaAvatarCollectionList.list = new List<MetaAvatarCollection>();
            availableMetaObjectCollectionList.list = new List<MetaObjectCollection>();


            realityMetaAvatarCollectionList = new List<MetaAvatarCollection>();
            realityMetaObjectCollectionList = new List<MetaObjectCollection>();

            foreach (var tag in avatarTags)
            {
                for (int i = 0; i < availableMetaAvatarCollectionList.list.Count; i++)
                {
                    if (availableMetaAvatarCollectionList.list[i].tags.Contains(tag))
                    {
                        Debug.Log(availableMetaAvatarCollectionList.list[i].name + "Contains Tag: " + tag.name + ", add to List");
                        realityMetaAvatarCollectionList.Add(availableMetaAvatarCollectionList.list[i]);
                    }
                    else
                    {
                        Debug.Log(availableMetaAvatarCollectionList.list[i].name + "Do not Contains Tag: " + tag.name + "");
                    }
                }
            }

            foreach (var tag in objectTags)
            {
                for (int i = 0; i < availableMetaObjectCollectionList.list.Count; i++)
                {
                    if (availableMetaObjectCollectionList.list[i].tags.Contains(tag))
                    {
                        Debug.Log(availableMetaObjectCollectionList.list[i].name + "Contains Tag: " + tag.name + ", add to List");

                        realityMetaObjectCollectionList.Add(availableMetaObjectCollectionList.list[i]);
                    }
                    else
                    {
                        Debug.Log(availableMetaObjectCollectionList.list[i].name + "Do not Contains Tag: " + tag.name + "");
                    }
                }
            }
        }
    }
}