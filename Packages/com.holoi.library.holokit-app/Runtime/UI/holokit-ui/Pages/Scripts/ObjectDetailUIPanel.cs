using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    [ExecuteInEditMode]
    public class ObjectDetailUIPanel : MonoBehaviour
    {
        [Header("Data")]
        public MetaObject metaObject;
        public MetaAvatar metaAvatar;
        [Header("Theme")]
        public Theme theme;
        [Header("UI Elements - Text")]
        [SerializeField] Image _backGround;
        [SerializeField] FlexibleUIText _ID;
        [SerializeField] FlexibleUIText _objectName;
        [SerializeField] FlexibleUIText _collected;
        [SerializeField] FlexibleUIText _notCollected;
        [SerializeField] FlexibleUIText _collectionName;
        [SerializeField] FlexibleUIText _author;
        [SerializeField] FlexibleUIText _description;
        [SerializeField] FlexibleUIText _siteAddress;
        [Header("UI Elements - Button")]
        [SerializeField] FlexibleUIBackButton _backButton;
        [SerializeField] FlexibleUIPlayButton _playButton;
        [SerializeField] FlexibleUIIconButton _openSeaButton;



        public enum Theme
        {
            dark = 0,
            bright = 1,
        }

        // this is for debug, will get state from data
        public enum CollectionState
        {
            collected,
            notCollected

        }

       [HideInInspector] public CollectionState state = CollectionState.collected;
        


        private void Start()
        {
            InitUIInfo();
            SetCollectState(state);
            switch (theme)
            {
                case Theme.dark:
                    _backGround.color = Color.black;
                    _ID.color = FlexibleUIText.HolokitColor.White;
                    _objectName.color = FlexibleUIText.HolokitColor.White;
                    _collected.color = FlexibleUIText.HolokitColor.White;
                    _notCollected.color = FlexibleUIText.HolokitColor.White;
                    _collectionName.color = FlexibleUIText.HolokitColor.White;
                    _author.color = FlexibleUIText.HolokitColor.White;
                    _description.color = FlexibleUIText.HolokitColor.White;
                    _siteAddress.color = FlexibleUIText.HolokitColor.White;

                    _backButton.theme = FlexibleUIBackButton.Theme.Dark;
                    _playButton.theme = FlexibleUIPlayButton.Theme.Dark;
                    _openSeaButton.theme = FlexibleUIIconButton.Theme.Dark;
                    break;
                case Theme.bright:
                    _backGround.color = Color.white;
                    _ID.color = FlexibleUIText.HolokitColor.Black;
                    _objectName.color = FlexibleUIText.HolokitColor.Black;
                    _collected.color = FlexibleUIText.HolokitColor.Black;
                    _notCollected.color = FlexibleUIText.HolokitColor.Black;
                    _collectionName.color = FlexibleUIText.HolokitColor.Black;
                    _author.color = FlexibleUIText.HolokitColor.Black;
                    _description.color = FlexibleUIText.HolokitColor.Black;
                    _siteAddress.color = FlexibleUIText.HolokitColor.Black;

                    _backButton.theme = FlexibleUIBackButton.Theme.Bright;
                    _playButton.theme = FlexibleUIPlayButton.Theme.Bright;
                    _openSeaButton.theme = FlexibleUIIconButton.Theme.Bright;
                    break;
            }
            // get collect state from data:
            //state
        }

        private void Update()
        {

        }

        void InitUIInfo()
        {
            if (metaObject)
            {
                _ID.text.text = "#" + metaObject.tokenId;
                _objectName.text.text = metaObject.name;
                _collectionName.text.text = metaObject.collection.displayName;
                _author.text.text = metaObject.collection.author;
                _description.text.text = metaObject.collection.description;
                _description.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    1086,
                    _description.text.preferredHeight);

                //_siteAddress.text.text = metaObject.collection.siteAddress;
            }
            else
            {
                _ID.text.text = "#" + metaAvatar.tokenId;
                _objectName.text.text = metaAvatar.name;
                _collectionName.text.text = metaAvatar.collection.displayName;
                _author.text.text = metaAvatar.collection.author;
                _description.text.text = metaAvatar.collection.description;
                _description.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    1086,
                    _description.text.preferredHeight);

                //_siteAddress.text.text = metaAvatar.collection.siteAddress;
            }

        }

        void SetCollectState(CollectionState state)
        {
            this.state = state;
            switch (this.state)
            {
                case CollectionState.collected:
                    _collected.gameObject.SetActive(true);
                    _notCollected.gameObject.SetActive(false);
                    break;
                case CollectionState.notCollected:
                    _collected.gameObject.SetActive(false);
                    _notCollected.gameObject.SetActive(true);
                    break;
            }
        }
    }
}