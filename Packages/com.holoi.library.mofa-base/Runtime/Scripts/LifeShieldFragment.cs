using UnityEngine;

namespace Holoi.Library.MOFABase
{
    public enum LifeShieldArea
    {
        Center = 0,
        Top = 1,
        Left = 2,
        Right = 3
    }

    public class LifeShieldFragment : MonoBehaviour
    {
        public LifeShieldArea Area;
    }
}