using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIPanel_MonoAR_RealitySettings : HoloKitAppUIPanel
    {
        public override string UIPanelName => "MonoAR_RealitySettings";

        public override bool OverlayPreviousPanel => true;

        [SerializeField] private RectTransform _exitButton;

        [SerializeField] private HoloKitAppUIRealitySettingTabSelector _tabSelectorPrefab;

        [SerializeField] private RectTransform _tabSelectorRoot;

        [SerializeField] private List<HoloKitAppUIRealitySettingTab> _defaultUIRealitySettingTabs;

        private HoloKitAppUIRealitySettingTab _currentTab;

        private List<HoloKitAppUIRealitySettingTabSelector> _tabSelectors;

        private const float ScreenWidth = 1170f;

        private void Start()
        {
            GetComponent<RectTransform>().anchoredPosition = new(0f, 0f);
            _tabSelectors = new();
            List<HoloKitAppUIRealitySettingTab> tabList = new(_defaultUIRealitySettingTabs);
            var realityManager = HoloKitApp.Instance.RealityManager;
            tabList.AddRange(realityManager.Config.UIRealitySettingTabPrefabs);
            tabList = tabList.Distinct().ToList();
            foreach (var tab in tabList)
            {
                if (tab.TabName.Equals("Adjust"))
                {
                    // If there is no ARObjectAdjuster in the scene
                    if (ARObjectAdjuster.Instance == null)
                    {
                        continue;
                    }
                }
                var tabSelector = Instantiate(_tabSelectorPrefab);
                tabSelector.Init(tab.TabName, () =>
                {
                    if (tabSelector.IsSelected) { return; }
                    UnselectAllTabSelectors();
                    tabSelector.OnSelected();
                    UpdateSelectedTab(tab);
                });
                tabSelector.GetComponent<RectTransform>().sizeDelta =
                    new(ScreenWidth / tabList.Count, _tabSelectorRoot.sizeDelta.y);
                tabSelector.transform.SetParent(_tabSelectorRoot);
                tabSelector.transform.localScale = Vector3.one;
                _tabSelectors.Add(tabSelector);
            }
            _tabSelectors[0].OnTabSelected();
        }

        private void UpdateSelectedTab(HoloKitAppUIRealitySettingTab realitySettingTab)
        {
            if (_currentTab != null)
            {
                Destroy(_currentTab.gameObject);
            }
            _currentTab = Instantiate(realitySettingTab);
            _currentTab.transform.SetParent(transform);
            var tabRectTransform = _currentTab.GetComponent<RectTransform>();
            tabRectTransform.anchoredPosition = new(0f, _tabSelectorRoot.sizeDelta.y);
            //GetComponent<RectTransform>().sizeDelta = new(ScreenWidth, Mathf.Max(_tabSelectorRoot.sizeDelta.y, tabRectTransform.sizeDelta.y));
        }

        private void UnselectAllTabSelectors()
        {
            foreach (var tabSelector in _tabSelectors)
            {
                tabSelector.OnUnselected();
            }
        }

        public void OnExitButtonPressed()
        {
            HoloKitApp.Instance.UIPanelManager.PopUIPanel();
        }
    }
}
