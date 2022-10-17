using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    public abstract class HoloKitAppUITemplate_RealityPreferencesPage_ChooseArtifact : MonoBehaviour
    {
        [Header("Artifact Collection Selector")]
        [SerializeField] private RectTransform _artifactCollectionScrollRoot;

        [SerializeField] private HoloKitAppUIComponent_RealityPreferencesPage_ArtifactCollectionTab _artifactCollectionTabPrefab;

        protected ArtifactCollection CurrentArtifactCollection;

        [Header("Artifact Selector")]
        [SerializeField] private TMP_Text _artifactTokenId;

        [SerializeField] private TMP_Text _artifactCollectionName;

        [SerializeField] private RectTransform _artifactScrollRoot;

        [SerializeField] private Image _artifactSlotPrefab;

        [SerializeField] private Sprite _artifactDefaultImage;

        [SerializeField] private Scrollbar _artifactHorizontalScrollbar;

        private int _currentArtifactIndex = -1;

        private float _artifactHorizontalScrollbarInterval = 1f;

        private const float ArtifactHorizontalScrollbarMaxDeviation = 0.001f;

        private const float ArtifactHorizontalScrollbarMovementSpeed = 1f;

        private void Start()
        {
            var compatibleArtifactCollectionList = GetCompatibleArtifactCollectionList();
            if (compatibleArtifactCollectionList.Count == 0)
            {
                // Current reality does not need avatar
                Destroy(gameObject);
                return;
            }

            for (int i = 0; i < _artifactCollectionScrollRoot.childCount; i++)
            {
                Destroy(_artifactCollectionScrollRoot.GetChild(i).gameObject);
            }

            // Setup avatar collection list
            foreach (var artifactCollection in compatibleArtifactCollectionList)
            {
                var tabInstance = Instantiate(_artifactCollectionTabPrefab);
                tabInstance.transform.SetParent(_artifactCollectionScrollRoot);
                tabInstance.transform.localScale = Vector3.one;
                tabInstance.Init(artifactCollection.DisplayName, artifactCollection, OnNewTabSelected);

                // Select the current preferenced avatar collection
                if (artifactCollection.BundleId.Equals(HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.BundleId].MetaAvatarCollectionBundleId))
                {
                    tabInstance.OnSelected();
                }
            }

            CurrentArtifactCollection = HoloKitApp.Instance.GlobalSettings.GetPreferencedMetaAvatarCollection(HoloKitApp.Instance.CurrentReality);
            UpdateArtifactSelector(CurrentArtifactCollection);
            // Scroll to the preferenced avatar image
            int preferencedArtifactIndex = GetPreferencedArtifactIndex();
            StartCoroutine(SetArtifactScrollbarValue(preferencedArtifactIndex * _artifactHorizontalScrollbarInterval));
        }

        private void Update()
        {
            if (Input.touchCount > 0) { return; }

            float deviation = _artifactHorizontalScrollbar.value % _artifactHorizontalScrollbarInterval;
            if (Mathf.Abs(deviation) > ArtifactHorizontalScrollbarMaxDeviation)
            {
                if (deviation > _artifactHorizontalScrollbarInterval / 2f)
                {
                    _artifactHorizontalScrollbar.value += ArtifactHorizontalScrollbarMovementSpeed * Time.deltaTime;
                }
                else
                {
                    _artifactHorizontalScrollbar.value -= ArtifactHorizontalScrollbarMovementSpeed * Time.deltaTime;
                }
            }
            else
            {
                int index = Mathf.RoundToInt(_artifactHorizontalScrollbar.value / _artifactHorizontalScrollbarInterval);
                if (index != _currentArtifactIndex)
                {
                    _currentArtifactIndex = index;
                    UpdateRealityPreferences(CurrentArtifactCollection.BundleId, CurrentArtifactCollection.Artifacts[_currentArtifactIndex].TokenId);
                }
            }
        }

        protected abstract List<ArtifactCollection> GetCompatibleArtifactCollectionList();

        protected abstract int GetPreferencedArtifactIndex();

        private void OnNewTabSelected(ArtifactCollection artifactCollection)
        {
            CurrentArtifactCollection = artifactCollection;
            for (int i = 0; i < _artifactCollectionScrollRoot.childCount; i++)
            {
                var artifactCollectionTab = _artifactCollectionScrollRoot.GetChild(i).GetComponent<HoloKitAppUIComponent_RealityPreferencesPage_ArtifactCollectionTab>();
                if (artifactCollectionTab.ArtifactCollection.Equals(artifactCollection))
                {
                    artifactCollectionTab.OnSelected();
                }
                else
                {
                    artifactCollectionTab.OnUnselected();
                }
            }

            UpdateArtifactSelector(artifactCollection);

            int coverArtifactIndex = artifactCollection.GetArtifactIndex(artifactCollection.CoverArtifact.TokenId);
            StartCoroutine(SetArtifactScrollbarValue(coverArtifactIndex * _artifactHorizontalScrollbarInterval));

            UpdateRealityPreferences(artifactCollection.BundleId, artifactCollection.CoverArtifact.TokenId);
        }

        private IEnumerator SetArtifactScrollbarValue(float value)
        {
            // Wait for 1 frame
            yield return null;
            _artifactHorizontalScrollbar.value = value;
        }

        private void UpdateArtifactSelector(ArtifactCollection artifactCollection)
        {
            _artifactTokenId.text = "#" + artifactCollection.CoverArtifact.TokenId;
            _artifactCollectionName.text = artifactCollection.DisplayName;

            // Destroy previous avatar portratis
            for (int i = 0; i < _artifactScrollRoot.childCount; i++)
            {
                Destroy(_artifactScrollRoot.GetChild(i).gameObject);
            }

            foreach (var artifact in artifactCollection.Artifacts)
            {
                Image slotInstance = Instantiate(_artifactSlotPrefab);
                slotInstance.transform.SetParent(_artifactScrollRoot);
                slotInstance.transform.localScale = Vector3.one;
                if (artifact.Image == null)
                {
                    slotInstance.sprite = _artifactDefaultImage;
                }
                else
                {
                    slotInstance.sprite = artifact.Image;
                }
            }

            _artifactHorizontalScrollbarInterval = 1f / (artifactCollection.Artifacts.Count - 1);
        }

        protected abstract void UpdateRealityPreferences(string artifactCollectionId, string artifactTokenId);
    }
}
