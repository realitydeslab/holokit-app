using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Mofa.Base
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MagicSchool")]
    public class MagicSchool : ScriptableObject
    {
        public int Id;

        public string Name;
    }
}