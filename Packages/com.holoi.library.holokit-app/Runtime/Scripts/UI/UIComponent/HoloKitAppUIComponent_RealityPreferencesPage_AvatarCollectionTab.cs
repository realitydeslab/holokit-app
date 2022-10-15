using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityPreferencesPage_AvatarCollectionTab : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private TMP_Text _tabName;

        [SerializeField] private Image _tabImage;

        private bool _selected;

        private string _avatarCollectionId;

        private Action _refreshAllTabs;

        private readonly Color WhiteColor = new(1f, 1f, 1f, 1f);

        private readonly Color BlackColor = new(0f, 0f, 0f, 1f);

        public void Init(string name, string avatarCollectionId, Action RefreshAllTabs)
        {
            _tabName.text = name;
            _avatarCollectionId = avatarCollectionId;
            _refreshAllTabs = RefreshAllTabs;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            // Avoid repeated tab
            if (_selected)
            {
                return;
            }

            OnSelected();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            
        }

        public void OnSelected()
        {
            _refreshAllTabs();
            _selected = true;
            // Update visual
            _tabName.color = WhiteColor;
            _tabImage.color = BlackColor;

            // Save the new avatar collection to global settings
            RealityPreference realityPreference = HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.id];
            string coverAvatarTokenId = HoloKitApp.Instance.GlobalSettings.GetMetaAvatarCollection(realityPreference.MetaAvatarCollectionId).coverMetaAvatar.tokenId;
            HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.id] = new RealityPreference()
            {
                MetaAvatarCollectionId = _avatarCollectionId,
                MetaAvatarTokenId = coverAvatarTokenId,
                MetaObjectCollectionId = realityPreference.MetaObjectCollectionId,
                MetaObjectTokenId = realityPreference.MetaObjectTokenId
            };
        }

        public void OnUnselected()
        {
            _selected = false;
            // Update visual
            _tabName.color = BlackColor;
            _tabImage.color = WhiteColor;
        }
    }
}
