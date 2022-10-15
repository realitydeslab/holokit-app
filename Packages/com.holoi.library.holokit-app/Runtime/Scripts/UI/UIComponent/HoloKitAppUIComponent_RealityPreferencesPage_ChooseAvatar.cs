using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealityPreferencesPage_ChooseAvatar : MonoBehaviour
    {
        [Header("Avatar Collection Selector")]
        [SerializeField] private HoloKitAppUIComponent_RealityPreferencesPage_AvatarCollectionTab _avatarCollectionTabPrefab;

        [SerializeField] private RectTransform _avatarCollectionScrollRoot;

        [Header("Avatar Selector")]
        [SerializeField] private TMP_Text _avatarTokenId;

        [SerializeField] private TMP_Text _avatarCollectionName;

        private void Start()
        {
            // TODO: Delete this line
            HoloKitApp.Instance.CurrentReality = HoloKitApp.Instance.GlobalSettings.RealityList.realities[0];

            var compatibleAvatarCollectionList = HoloKitApp.Instance.GlobalSettings.GetCompatibleMetaAvatarCollectionList(HoloKitApp.Instance.CurrentReality);
            if (compatibleAvatarCollectionList.Count == 0)
            {
                // Current reality does not need avatar
                Destroy(gameObject);
                return;
            }

            // Setup avatar collection list
            foreach (var avatarCollection in compatibleAvatarCollectionList)
            {
                var tabInstance = Instantiate(_avatarCollectionTabPrefab);
                tabInstance.transform.SetParent(_avatarCollectionScrollRoot);
                tabInstance.transform.localScale = Vector3.one;
                tabInstance.Init(avatarCollection.displayName, avatarCollection.id, RefreshAllTabs);

                // Select the current preferenced avatar collection
                if (avatarCollection.id.Equals(HoloKitApp.Instance.GlobalSettings.RealityPreferences[HoloKitApp.Instance.CurrentReality.id].MetaAvatarCollectionId))
                {
                    tabInstance.OnSelected();
                }
            }
        }

        private void RefreshAllTabs()
        {
            for (int i = 0; i < _avatarCollectionScrollRoot.childCount; i++)
            {
                _avatarCollectionScrollRoot.GetChild(i).GetComponent<HoloKitAppUIComponent_RealityPreferencesPage_AvatarCollectionTab>().OnUnselected();
            }
        }
    }
}
