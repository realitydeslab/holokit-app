using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityPreferencesPage_NonFungibleCollectionTab : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private TMP_Text _tabName;

        [SerializeField] private Image _tabImage;

        public bool IsSelected
        {
            get
            {
                return _tabName.color.Equals(Color.white) && _tabImage.color.Equals(Color.black);
            }
        }

        public NonFungibleCollection NonFungibleCollection => _nonFungibleCollection;

        private NonFungibleCollection _nonFungibleCollection;

        private Action<NonFungibleCollection> _onNewTabSelected;

        public void Init(string name, NonFungibleCollection nonFungibleCollection, Action<NonFungibleCollection> OnNewTabSelected)
        {
            _tabName.text = name;
            _nonFungibleCollection = nonFungibleCollection;
            _onNewTabSelected = OnNewTabSelected;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            // Avoid repeated tab
            if (IsSelected)
            {
                Debug.Log("Tab already selected");
                return;
            }

            _onNewTabSelected(_nonFungibleCollection);
        }

        public virtual void OnPointerUp(PointerEventData eventData) { }

        public void OnSelected()
        {
            // Update visual
            _tabName.color = Color.white;
            _tabImage.color = Color.black;
        }

        public void OnUnselected()
        {
            // Update visual
            _tabName.color = Color.black;
            _tabImage.color = Color.white;
        }
    }
}
