using UnityEngine;

namespace Holoi.Library.MOFABase
{
    public enum SpellType
    {
        Basic = 0,
        Secondary = 1
    }

    public class Spell : MonoBehaviour
    {
        public int Id;

        public string Name;

        public MagicSchool MagicSchool;

        public SpellType SpellType;

        public Vector3 SpawnOffset;

        public bool PerpendicularToGround;

        public float ChargeTime;

        public int MaxChargeCount;

        public int MaxUseCount;

        //private void Start()
        //{
        //    Debug.Log($"[Spell] {Name} spawned with ownership {GetComponent<Unity.Netcode.NetworkObject>().OwnerClientId}");
        //}
    }
}