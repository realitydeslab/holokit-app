using UnityEngine;
using TMPro;

namespace Holoi.Library.HoloKitApp
{
    public class HoloKitAppUIComponent_RealityDetailPage_Technology : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;

        [SerializeField] private GameObject _tagRowPrefab;

        [SerializeField] private GameObject _technologySlotPrefab;

        private float _currentAccumulatedSlotWidth;

        private const float WordCountToWidthRatio = 24f;

        private const float SlotHeight = 70;

        private const float AccumulatedSlotWidthThreshould = 700;

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
                technologySlot.GetComponentInChildren<TMP_Text>().text = "#" + realityTag.DisplayName;
                float slotWidth = realityTag.DisplayName.Length * WordCountToWidthRatio;
                _currentAccumulatedSlotWidth += slotWidth;
                technologySlot.GetComponent<RectTransform>().sizeDelta = new Vector2(slotWidth, SlotHeight);
            }
        }
    }
}
