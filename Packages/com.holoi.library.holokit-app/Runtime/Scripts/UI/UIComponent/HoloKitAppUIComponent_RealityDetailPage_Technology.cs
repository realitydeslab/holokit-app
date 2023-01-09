using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using TMPro;

namespace Holoi.Library.HoloKitApp
{
    public class HoloKitAppUIComponent_RealityDetailPage_Technology : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;

        [SerializeField] private GameObject _tagRowPrefab;

        [SerializeField] private GameObject _technologySlotPrefab;

        private float _currentAccumulatedSlotWidth;

        private const float SlotHeight = 70;

        private const float AccumulatedSlotWidthThreshould = 680;

        private readonly List<float> WordCountToSlotWidthList = new() { 0, 50, 100, 150, 180, 200, 220, 240, 260, 270, 280, // 10
                                                                        290, 300, 320, 330, 360, 380, 420, 450, 480, 500 }; // 20

        private void Start()
        {
            var currentRow = Instantiate(_tagRowPrefab);
            currentRow.transform.SetParent(_root);
            currentRow.transform.localScale = Vector3.one;
            _currentAccumulatedSlotWidth = 0f;
            foreach (var realityTag in HoloKitApp.Instance.CurrentReality.RealityTags)
            {
                if (_currentAccumulatedSlotWidth > AccumulatedSlotWidthThreshould)
                {
                    currentRow = Instantiate(_tagRowPrefab);
                    currentRow.transform.SetParent(_root);
                    currentRow.transform.localScale = Vector3.one;
                    _currentAccumulatedSlotWidth = 0f;
                }
                var technologySlot = Instantiate(_technologySlotPrefab);
                technologySlot.transform.SetParent(currentRow.transform);
                technologySlot.transform.localScale = Vector3.one;

                switch (LocalizationSettings.SelectedLocale.Identifier.Code)
                {
                    case "en":
                        technologySlot.GetComponentInChildren<TMP_Text>().text = "#" + realityTag.DisplayName;
                        break;
                    case "zh-Hans":
                        technologySlot.GetComponentInChildren<TMP_Text>().text = "#" + realityTag.DisplayName_Chinese;
                        break;
                }

                float slotWidth = WordCountToSlotWidthList[realityTag.DisplayName.Length];
                _currentAccumulatedSlotWidth += slotWidth;
                technologySlot.GetComponent<RectTransform>().sizeDelta = new Vector2(slotWidth, SlotHeight);
            }
        }
    }
}
