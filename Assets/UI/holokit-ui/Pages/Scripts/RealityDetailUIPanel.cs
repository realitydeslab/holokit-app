using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    [ExecuteInEditMode]
    public class RealityDetailUIPanel : MonoBehaviour
    {
        public Holoi.AssetFoundation.Reality reality;

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
            _contentInitPositionY = _content.position.y;
            _technicContainer.reality = reality;
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                if (PanelManager.Instance.GetActivePanel().UIType.Name == "RealityDetailPanel")
                {
                    UpdateThumbnailOffset();
                }
            }
            else
            {
                UpdateThumbnailOffset();
            }
        }

        public void UpdateInformation()
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

        void UpdateThumbnailOffset()
        {
            var deltaY = _content.transform.position.y - _contentInitPositionY;
            var realityThumbnailContainer = FindObjectOfType<RealityThumbnailContainer>();
            realityThumbnailContainer.offset = new Vector3(0, 1.7f + deltaY, 0);
        }
    }
}