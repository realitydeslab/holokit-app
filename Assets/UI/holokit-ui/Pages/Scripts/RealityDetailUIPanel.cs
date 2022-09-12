using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

namespace Holoi.HoloKit.App.UI
{
    [ExecuteInEditMode]
    public class RealityDetailUIPanel : MonoBehaviour
    {
        public Holoi.AssetFoundation.Reality reality;

        [SerializeField] Transform _content;
        [SerializeField] TMPro.TMP_Text _id;
        [SerializeField] TMPro.TMP_Text _name;
        [SerializeField] TMPro.TMP_Text _version;
        [SerializeField] TMPro.TMP_Text _lastUpdate;
        [SerializeField] TMPro.TMP_Text _author;
        [SerializeField] TMPro.TMP_Text _description;
        [SerializeField] TMPro.TMP_Text _technic;
        [SerializeField] Transform _videoContainer;

        float _contentInitPositionY;


        private void Awake()
        {
            _contentInitPositionY = _content.position.y;
        }

        private void Update()
        {
            if (PanelManager.Instance.GetActivePanel().UIType.Name == "RealityDetailPanel")
            {
                UpdateThumbnailOffset();
            }
        }

        public void UpdateInformation()
        {
            _id.text = "Reality #" + reality.realityId;
            _name.text = reality.name;
            _version.text = reality.version;
            _lastUpdate.text = "2022. 09. 28";
            _author.text = reality.author;
            _description.text = reality.description;
        }

        void UpdateThumbnailOffset()
        {
            var deltaY = _content.transform.position.y - _contentInitPositionY;
            var realityThumbnailContainer = FindObjectOfType<RealityThumbnailContainer>();
            realityThumbnailContainer._offset = new Vector3(0, 1.7f + deltaY, 0);
        }
    }
}