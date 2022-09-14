using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Mofa.Base
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MagicSchoolList")]
    public class MagicSchoolList : ScriptableObject
    {
        public List<MagicSchool> List;
    }
}