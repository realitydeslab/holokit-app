using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    public abstract class HoloKitAppUITemplate_RealityPreferencesPage_ChooseNonFungible : MonoBehaviour
    {
        [Header("Artifact Collection Selector")]
        [SerializeField] private RectTransform _nonFungibleCollectionScrollRoot;

        [SerializeField] private HoloKitAppUIComponent_RealityPreferencesPage_NonFungibleCollectionTab _nonFungibleCollectionTabPrefab;

        protected NonFungibleCollection CurrentNonFungibleCollection;

        [Header("Artifact Selector")]
        [SerializeField] private TMP_Text _nonFungibleTokenId;

        [SerializeField] private TMP_Text _nonFungibleCollectionName;

        [SerializeField] private RectTransform _nonFungibleScrollRoot;

        [SerializeField] private Image _nonFungibleSlotPrefab;

        [SerializeField] private Sprite _nonFungibleDefaultImage;

        private int _currentNonFungibleIndex = -1;

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
        }

        private void Update()
        {
            if (Input.touchCount > 0) { return; }

            int step = Mathf.Abs(Mathf.RoundToInt(_nonFungibleScrollRoot.anchoredPosition.x / _nonFungibleSlotPrefab.rectTransform.sizeDelta.x));
            float deviation = Mathf.Abs(_nonFungibleScrollRoot.anchoredPosition.x % _nonFungibleSlotPrefab.rectTransform.sizeDelta.x);
            if (deviation != 0f)
            {
                if (deviation < 30f)
                {
                    _nonFungibleScrollRoot.anchoredPosition = new Vector2(-step * _nonFungibleSlotPrefab.rectTransform.sizeDelta.x,
                                                                          _nonFungibleScrollRoot.anchoredPosition.y);
                    return;
                }

                if (deviation > _nonFungibleSlotPrefab.rectTransform.sizeDelta.x / 2f)
                {
                    _nonFungibleScrollRoot.anchoredPosition -= new Vector2(NonFungibleHorizontalScrollbarMovementSpeed * Time.deltaTime,
                                                                          _nonFungibleScrollRoot.anchoredPosition.y);
                }
                else
                {
                    _nonFungibleScrollRoot.anchoredPosition += new Vector2(NonFungibleHorizontalScrollbarMovementSpeed * Time.deltaTime,
                                                                          _nonFungibleScrollRoot.anchoredPosition.y);
                }
            }
            else
            {
                if (step != _currentNonFungibleIndex)
                {
                    //Debug.Log($"Changed to step: {step}");
                    _currentNonFungibleIndex = step;
                    UpdateRealityPreferences(CurrentNonFungibleCollection.BundleId, CurrentNonFungibleCollection.NonFungibles[_currentNonFungibleIndex].TokenId);
                    _nonFungibleTokenId.text = "#" + CurrentNonFungibleCollection.NonFungibles[step].TokenId;
                }
            }
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

            foreach (var artifact in nonFungibleCollection.NonFungibles)
            {
                Image slotInstance = Instantiate(_nonFungibleSlotPrefab);
                slotInstance.transform.SetParent(_nonFungibleScrollRoot);
                slotInstance.transform.localScale = Vector3.one;
                if (artifact.Image == null)
                {
                    slotInstance.sprite = _nonFungibleDefaultImage;
                }
                else
                {
                    slotInstance.sprite = artifact.Image;
                }
            }
        }
    }
}
