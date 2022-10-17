using UnityEngine;

namespace Holoi.AssetFoundation
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MetaObject")]
    public class MetaObject: NonFungible
    {
        public override NonFungibleCollection NonFungibleCollection => MetaObjectCollection;

        public MetaObjectCollection MetaObjectCollection;
    }
}