using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    [ExecuteInEditMode]
    public class ObjectDetailUIPanel : MonoBehaviour
    {
        public MetaObject metaObject;
        [SerializeField] TMPro.TMP_Text _ID;
        [SerializeField] TMPro.TMP_Text _collectionName;
        [SerializeField] TMPro.TMP_Text _objectName;
        [SerializeField] TMPro.TMP_Text _author;
        [SerializeField] TMPro.TMP_Text _description;
        [SerializeField] TMPro.TMP_Text _siteAddress;
        [Header("Collection Status")]
        [SerializeField] Transform _notCollectedUI;
        [SerializeField] Transform _collectedUI;

        public enum State
        {
            collected,
            notCollected

        }

        public State state;

        private void Awake()
        {
            _collectionName.text = metaObject.collection.displayName;
            _ID.text = "#" + metaObject.tokenId;
            _objectName.text = metaObject.name;
            _author.text = metaObject.collection.author;
            _description.text = metaObject.collection.description;

        }

        private void Update()
        {
            if (Application.isEditor)
            {
            }
        }

        void SetState(State s)
        {
            state = s;
            switch (state)
            {
                case State.collected:
                    _collectedUI.gameObject.SetActive(true);
                    _notCollectedUI.gameObject.SetActive(false);

                    break;
                case State.notCollected:
                    _collectedUI.gameObject.SetActive(false);
                    _notCollectedUI.gameObject.SetActive(true);
                    break;
            }
        }
    }
}