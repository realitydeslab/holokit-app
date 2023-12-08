// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    public abstract class HoloKitAppUITemplate_RealityPreferencesPage_ChooseNonFungible : MonoBehaviour
    {
        [SerializeField] protected TMP_Text NonFungibleDescription;

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

        [SerializeField] private RectTransform _nonFungibleImageArea;

        private bool _isTouching;

        private Vector3 _touchBeganPosition;

        private float _touchBeganTime;

        private int _currentNonFungibleIndex = -1;

        private const float CompleteLeanTweenDuration = 0.4f;

        private const float SwiftSwipeThreshold = 1200f;

        protected abstract List<NonFungibleCollection> GetCompatibleNonFungibleCollectionList();

        protected abstract NonFungibleCollection GetPreferencedNonFungibleCollection();

        protected abstract int GetPreferencedNonFungibleIndex();

        protected abstract void UpdateRealityPreferences(string artifactCollectionId, string artifactTokenId);

        protected abstract void UpdateNonFungibleDescription();

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
            _currentNonFungibleIndex = GetPreferencedNonFungibleIndex();
            float value = _currentNonFungibleIndex * _nonFungibleSlotPrefab.rectTransform.sizeDelta.x;
            _nonFungibleScrollRoot.anchoredPosition = new Vector2(-value, _nonFungibleScrollRoot.anchoredPosition.y);
            // Set the dots
            _nonFungibleSelectionIndicator.Init(_currentNonFungibleIndex, CurrentNonFungibleCollection.NonFungibles.Count);

            UpdateNonFungibleDescription();
        }

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    if (!IsInsideInputArea(touch.position))
                    {
                        _isTouching = false;
                        return;
                    }
                    _isTouching = true;
                    _touchBeganPosition = touch.position;
                    _touchBeganTime = Time.time;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    if (!_isTouching) { return; }

                    _isTouching = false;
                    LeanTween.cancel(_nonFungibleScrollRoot);

                    // If this is a swift swipe
                    float swipeX = touch.position.x - _touchBeganPosition.x;
                    if (Mathf.Abs(swipeX) / (Time.time - _touchBeganTime) > SwiftSwipeThreshold)
                    {
                        // This is a swift swipe
                        _currentNonFungibleIndex = swipeX > 0 ? _currentNonFungibleIndex - 1 : _currentNonFungibleIndex + 1;
                        if (_currentNonFungibleIndex < 0)
                        {
                            _currentNonFungibleIndex = 0;
                        }
                        else if (_currentNonFungibleIndex > CurrentNonFungibleCollection.NonFungibles.Count - 1)
                        {
                            _currentNonFungibleIndex = CurrentNonFungibleCollection.NonFungibles.Count - 1;
                        }
                    }
                    else
                    {
                        // This is not a swift swipe
                        _currentNonFungibleIndex = Mathf.RoundToInt(-_nonFungibleScrollRoot.anchoredPosition.x / _nonFungibleSlotPrefab.rectTransform.sizeDelta.x);
                    }
                    float diff = Mathf.Abs(_nonFungibleScrollRoot.anchoredPosition.x + _currentNonFungibleIndex * _nonFungibleSlotPrefab.rectTransform.sizeDelta.x);
                    float leanTweenDuration = diff / _nonFungibleSlotPrefab.rectTransform.sizeDelta.x * CompleteLeanTweenDuration;
                    LeanTween.moveX(_nonFungibleScrollRoot, -_currentNonFungibleIndex * _nonFungibleSlotPrefab.rectTransform.sizeDelta.x, leanTweenDuration)
                        .setEase(LeanTweenType.easeOutCubic)
                        .setOnComplete(() =>
                        {
                            OnCurrentNonFungibleChanged();
                        });
                }
            }
        }

        private void OnCurrentNonFungibleChanged()
        {
            _nonFungibleTokenId.text = "#" + CurrentNonFungibleCollection.NonFungibles[_currentNonFungibleIndex].TokenId;
            // Set the dots
            _nonFungibleSelectionIndicator.UpdateIndex(_currentNonFungibleIndex);
            UpdateRealityPreferences(CurrentNonFungibleCollection.BundleId, CurrentNonFungibleCollection.NonFungibles[_currentNonFungibleIndex].TokenId);
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

            _currentNonFungibleIndex = nonFungibleCollection.GetNonFungibleIndex(nonFungibleCollection.CoverNonFungible.TokenId);
            float value = _currentNonFungibleIndex * _nonFungibleSlotPrefab.rectTransform.sizeDelta.x;
            _nonFungibleScrollRoot.anchoredPosition = new Vector2(-value, _nonFungibleScrollRoot.anchoredPosition.y);
            // TODO: I don't know the reason, but if I didn't do this, the anchorPosition would be 0 on x
            StartCoroutine(HoloKitAppUtils.WaitAndDo(0f, () =>
            {
                _nonFungibleScrollRoot.anchoredPosition = new Vector2(-value, _nonFungibleScrollRoot.anchoredPosition.y);
            }));
            // Set the dots
            _nonFungibleSelectionIndicator.Init(_currentNonFungibleIndex, CurrentNonFungibleCollection.NonFungibles.Count);
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

        private bool IsInsideInputArea(Vector2 position)
        {
            if (position.x > (_nonFungibleImageArea.position.x - _nonFungibleImageArea.sizeDelta.x / 2f)
                && position.x < (_nonFungibleImageArea.position.x + _nonFungibleImageArea.sizeDelta.x / 2f)
                &&
                position.y > (_nonFungibleImageArea.position.y - _nonFungibleImageArea.sizeDelta.y / 2f)
                && position.y < (_nonFungibleImageArea.position.y + _nonFungibleImageArea.sizeDelta.y / 2f))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
