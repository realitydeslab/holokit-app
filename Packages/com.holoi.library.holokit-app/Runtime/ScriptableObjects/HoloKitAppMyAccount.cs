using UnityEngine;

namespace Holoi.HoloKit.App
{
    [CreateAssetMenu(menuName = "ScriptableObjects/HoloKitAppGlobalSettings")]
    public class HoloKitAppGlobalSettings : ScriptableObject
    {
        public string AppleId;

        public string WalletConnected;

        public string VerifiedHoloKit;

        public List<SoulboundToken> syncedSoulboundToken;

        public List<AvatarObject> syncedAvatars; 
        
        public List<MetaObject> syncedMetaObjects;

        private string SavedPath => Application.persistentDataPath + "/HoloKitAppGlobalSettings.save";
    }
}