using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    public abstract class HoloKitAppUITemplate_RealityPreferencesPage_ChooseNonFungible : MonoBehaviour
    {
        [Header("Non-Fungible Collection Selector")]
        [SerializeField] private RectTransform _nonFungibleCollectionScrollRoot;

        [SerializeField] private HoloKitAppUIComponent_RealityPreferencesPage_NonFungibleCollectionTab _nonFungibleCollectionTabPrefab;

        protected NonFungibleCollection CurrentNonFungibleCollection;

        [Header("Non-Fungible Selector")]
        [SerializeField] private TMP_Text _nonFungibleTokenId;

        [SerializeField] private TMP_Text _nonFungibleCollectionName;

        [SerializeField] private RectTransform _nonFungibleScrollRoot;

        [SerializeField] private Image _nonFungibleSlotPrefab;

        [SerializeField] private Sprite _nonFungibleDefaultImage;

        [SerializeField] private HoloKitAppUIComponent_RealityPreferencesPage_NonFungibleSelectionIndicator _nonFungibleSelectionIndicator;

        private int _currentNonFungibleIndex = -1;

        private const float CompleteLeanTweenDuration = 0.8f;

        private const float NonFungibleHorizontalScrollbarMovementSpeed = 400f;

        protected abstract List<NonFungibleCollection> GetCompatibleNonFungibleCollectionList();

        protected abstract NonFungibleCollection GetPreferencedNonFungibleCollection();

        protected abstract int GetPreferencedNonFungibleIndex();

        protected abstract void UpdateRealityPreferences(string artifactCollectionId, string artifactTokenId);

        private void Start()
        {
            var compatibleNonFungibleCollectionList = GetCompatibleNonFungibleCollectionList();
            if (compatibleNonFungibleCollectionList.Count == 0)
            {
                Destroy(gameObject);
                return;
            }

            for (int i = 0; i < _nonFungibleCollectionScrollRoot.childCount; i++)
            {
                Destroy(_nonFungibleCollectionScrollRoot.GetChild(i).gameObject);
            }

            CurrentNonFungibleCollection = GetPreferencedNonFungibleCollection();
            // Setup avatar collection list
            foreach (var nonFungibleCollection in compatibleNonFungibleCollectionList)
            {
                var tabInstance = Instantiate(_nonFungibleCollectionTabPrefab);
                tabInstance.transform.SetParent(_nonFungibleCollectionScrollRoot);
                tabInstance.transform.localScale = Vector3.one;
                tabInstance.Init(nonFungibleCollection.DisplayName, nonFungibleCollection, OnNewTabSelected);

                // Select the current preferenced avatar collection
                if (nonFungibleCollection.BundleId.Equals(CurrentNonFungibleCollection.BundleId))
                {
                    tabInstance.OnSelected();
                }
            }
            UpdateNonFungibleSelector(CurrentNonFungibleCollection);
            // Scroll to the preferenced avatar image
            int preferencedNonfungibleIndex = GetPreferencedNonFungibleIndex();
            float value = preferencedNonfungibleIndex * _nonFungibleSlotPrefab.rectTransform.sizeDelta.x;
            _nonFungibleScrollRoot.anchoredPosition = new Vector2(-value, _nonFungibleScrollRoot.anchoredPosition.y);
            // Set the dots
            _nonFungibleSelectionIndicator.Init(preferencedNonfungibleIndex, CurrentNonFungibleCollection.NonFungibles.Count);
        }

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                LeanTween.cancel(_nonFungibleScrollRoot);
                return;
            }

            if (LeanTween.isTweening(_nonFungibleScrollRoot)) { return; }
            float deviation = Mathf.Abs(_nonFungibleScrollRoot.anchoredPosition.x % _nonFungibleSlotPrefab.rectTransform.sizeDelta.x);
            if (deviation != 0)
            {
                // We are not at the center position of an image
                int step = Mathf.Abs(Mathf.RoundToInt(_nonFungibleScrollRoot.anchoredPosition.x / _nonFungibleSlotPrefab.rectTransform.sizeDelta.x));
                float leanTweenDuration = deviation / _nonFungibleSlotPrefab.rectTransform.sizeDelta.x * CompleteLeanTweenDuration;
                LeanTween.moveX(_nonFungibleScrollRoot, -step * _nonFungibleSlotPrefab.rectTransform.sizeDelta.x, leanTweenDuration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setOnComplete(() =>
                    {
                        _currentNonFungibleIndex = step;
                        _nonFungibleTokenId.text = "#" + CurrentNonFungibleCollection.NonFungibles[step].TokenId;
                        // Set the dots
                        _nonFungibleSelectionIndicator.UpdateIndex(_currentNonFungibleIndex);
                        UpdateRealityPreferences(CurrentNonFungibleCollection.BundleId, CurrentNonFungibleCollection.NonFungibles[_currentNonFungibleIndex].TokenId);
                    });
            }
            // TODO: If the user swipe right onto the center of an image
        }

        private void OnNewTabSelected(NonFungibleCollection nonFungibleCollection)
        {
            CurrentNonFungibleCollection = nonFungibleCollection;
            for (int i = 0; i < _nonFungibleCollectionScrollRoot.childCount; i++)
            {
                var nonFungibleCollectionTab = _nonFungibleCollectionScrollRoot.GetChild(i).GetComponent<HoloKitAppUIComponent_RealityPreferencesPage_NonFungibleCollectionTab>();
                if (nonFungibleCollectionTab.NonFungibleCollection.Equals(nonFungibleCollection))
                {
                    nonFungibleCollectionTab.OnSelected();
                }
                else
                {
                    nonFungibleCollectionTab.OnUnselected();
                }
            }

            UpdateNonFungibleSelector(nonFungibleCollection);

            int coverNonFungibleIndex = nonFungibleCollection.GetNonFungibleIndex(nonFungibleCollection.CoverNonFungible.TokenId);
            float value = coverNonFungibleIndex * _nonFungibleSlotPrefab.rectTransform.sizeDelta.x;
            _nonFungibleScrollRoot.anchoredPosition = new Vector2(-value, _nonFungibleScrollRoot.anchoredPosition.y);
            // TODO: I don't know the reason, but if I didn't do this, the anchorPosition would be 0 on x
            StartCoroutine(HoloKitAppUtils.WaitAndDo(0f, () =>
            {
                _nonFungibleScrollRoot.anchoredPosition = new Vector2(-value, _nonFungibleScrollRoot.anchoredPosition.y);
            }));
            // Set the dots
            _nonFungibleSelectionIndicator.Init(coverNonFungibleIndex, CurrentNonFungibleCollection.NonFungibles.Count);
            UpdateRealityPreferences(nonFungibleCollection.BundleId, nonFungibleCollection.CoverNonFungible.TokenId);
        }

        private void UpdateNonFungibleSelector(NonFungibleCollection nonFungibleCollection)
        {
            _nonFungibleTokenId.text = "#" + nonFungibleCollection.CoverNonFungible.TokenId;
            _nonFungibleCollectionName.text = nonFungibleCollection.DisplayName;

            // Destroy previous avatar portratis
            for (int i = 0; i < _nonFungibleScrollRoot.childCount; i++)
            {
                Destroy(_nonFungibleScrollRoot.GetChild(i).gameObject);
            }

            foreach (var nonFungible in nonFungibleCollection.NonFungibles)
            {
                Image slotInstance = Instantiate(_nonFungibleSlotPrefab);
                slotInstance.transform.SetParent(_nonFungibleScrollRoot);
                slotInstance.transform.localScale = Vector3.one;
                if (nonFungible.Image == null)
                {
                    slotInstance.sprite = _nonFungibleDefaultImage;
                }
                else
                {
                    slotInstance.sprite = nonFungible.Image;
                }
            }
        }
    }
}
