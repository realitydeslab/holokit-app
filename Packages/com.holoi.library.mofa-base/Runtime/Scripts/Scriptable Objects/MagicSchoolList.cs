using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.MOFABase
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MagicSchoolList")]
    public class MagicSchoolList : ScriptableObject
    {
        public List<MagicSchool> List;
    }
}