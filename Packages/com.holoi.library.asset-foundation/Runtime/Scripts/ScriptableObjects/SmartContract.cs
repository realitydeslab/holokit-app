using UnityEngine;
using Holoi.AssetFoundation;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/SmartContract")]
    public class SmartContract : ScriptableObject
    {
        public string address;
        public string chainName = "ethereum";

        public string OpenseaUrl(string tokenId) {
            return $"https://opensea.io/assets/{chainName}/{address}/{tokenId}";
        }
    }
}