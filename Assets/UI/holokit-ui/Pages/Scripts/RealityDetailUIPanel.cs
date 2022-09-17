using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    [ExecuteInEditMode]
    public class RealityDetailUIPanel : MonoBehaviour
    {
        public AssetFoundation.Reality reality;
        MetaObjectCollectionList availableMetaObjectCollectionList;
        MetaAvatarCollectionList availableMetaAvatarCollectionList;

        MetaObjectCollectionList realityMetaObjectCollectionList;
        MetaAvatarCollectionList realityMetaAvatarCollectionList;

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
            _technicContainer.reality = reality;
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
            _id.text.text = "Reality #" + reality.realityId;
            _name.text.text = reality.name;
            _version.text.text = reality.version;
            _author.text.text = reality.author;
            _description.text.text = reality.description;
            _description.GetComponent<RectTransform>().sizeDelta = new Vector2(
                1002,
                _description.text.preferredHeight);
        }

        void UpdateThumbnailPosition()
        {
            var deltaY = _content.transform.position.y - _contentInitPositionY;
            var realityThumbnailContainer = FindObjectOfType<RealityThumbnailContainer>();
            realityThumbnailContainer.scrollOffset = new Vector3(0, deltaY, 0);
            realityThumbnailContainer.rotateValue = _rotateHelperScrollBar.value;
        }

        void GetRealityCollections()
        {
            var objectTags = reality.compatibleMetaObjectTags;
            var avatarTags = reality.compatibleMetaAvatarTags;

            foreach (var tag in avatarTags)
            {
                for (int i = 0; i < availableMetaAvatarCollectionList.list.Count; i++)
                {
                    if (availableMetaAvatarCollectionList.list[i].tags.Contains(tag))
                    {
                        realityMetaAvatarCollectionList.list.Add(availableMetaAvatarCollectionList.list[i]);
                    }
                }
            }

            foreach (var tag in objectTags)
            {
                for (int i = 0; i < availableMetaObjectCollectionList.list.Count; i++)
                {
                    if (availableMetaObjectCollectionList.list[i].tags.Contains(tag))
                    {
                        realityMetaObjectCollectionList.list.Add(availableMetaObjectCollectionList.list[i]);
                    }
                }
            }
        }
    }
}