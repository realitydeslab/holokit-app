using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_TestRealityList : HoloKitAppUIPanel
    {
        public override string UIPanelName => "TestRealityList";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private HoloKitAppUIComponent_RealitySlot _realitySlotPrefab;

        [SerializeField] private RectTransform _listRoot;

        private void Start()
        {
            for (int i = 0; i < _listRoot.childCount; i++)
            {
                Destroy(_listRoot.GetChild(i).gameObject);
            }

            int realityIndex = 0;
            foreach (var reality in HoloKitApp.Instance.GlobalSettings.TestRealityList.List)
            {
                realityIndex++;
                var realitySlotInstance = Instantiate(_realitySlotPrefab);
                realitySlotInstance.transform.SetParent(_listRoot);
                realitySlotInstance.transform.localPosition = Vector3.zero;
                realitySlotInstance.transform.localScale = Vector3.one;
                realitySlotInstance.Init(reality, realityIndex);
            }
        }
    }
}
