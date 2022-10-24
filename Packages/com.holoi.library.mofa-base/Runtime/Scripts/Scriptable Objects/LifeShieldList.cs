using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.MOFABase
{
    [CreateAssetMenu(menuName = "ScriptableObjects/LifeShieldList")]
    public class LifeShieldList : ScriptableObject
    {
        public List<LifeShield> List;

        public LifeShield DefaultLifeShield;

        public LifeShield GetLifeShield(string magicSchoolTokenId)
        {
            foreach (var lifeShield in List)
            {
                if (lifeShield.MagicSchool.TokenId.Equals(magicSchoolTokenId))
                {
                    return lifeShield;
                }
            }
            return DefaultLifeShield;
        }
    }
}
