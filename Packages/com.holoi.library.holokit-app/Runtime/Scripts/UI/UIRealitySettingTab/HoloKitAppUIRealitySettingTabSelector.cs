using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIRealitySettingTabSelector : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private TMP_Text _tabName;

        [SerializeField] private Image _arrowImage;

        public bool IsSelected => _tabName.color.Equals(Color.white);

        public Action OnTabSelected;

        public void Init(string tabName, Action onTabSelected)
        {
            _tabName.text = tabName;
            OnTabSelected = onTabSelected;
            OnUnselected();
        } 

        public void OnPointerDown(PointerEventData eventData)
        {
            OnTabSelected?.Invoke();
        }

        public void OnSelected()
        {
            _tabName.color = Color.white;
            _arrowImage.color = Color.white;
        }

        public void OnUnselected()
        {
            _tabName.color = new Color(255f, 255f, 255f, 255f / 2f);
            _arrowImage.color = new Color(255f, 255f, 255f, 255f / 2f);
        }
    }
}
