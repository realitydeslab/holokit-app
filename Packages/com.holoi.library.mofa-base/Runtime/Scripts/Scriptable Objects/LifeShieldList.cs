using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.MOFABase
{
    [CreateAssetMenu(menuName = "ScriptableObjects/LifeShieldList")]
    public class LifeShieldList : ScriptableObject
    {
        public List<LifeShield> List;

        public LifeShield DefaultLifeShield;

        public LifeShield GetLifeShield(int index)
        {
            foreach (var lifeShield in List)
            {
                if (int.Parse(lifeShield.MagicSchool.TokenId) == index)
                {
                    return lifeShield;
                }
            }
            return DefaultLifeShield;
        }
    }
}
