using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Holoi.Library.MOFABase
{
    public class MofaSpellPool : NetworkBehaviour
    {
        [Tooltip("The number of basic spells pre-spawned for a player.")]
        [SerializeField] private int _basicSpellPrewarmCount = 6;

        [Tooltip("The number of secondary spells pre-spawned for a player.")]
        [SerializeField] private int _secondarySpellPrewarmCount = 2;

        private readonly Dictionary<GameObject, Queue<NetworkObject>> _pooledSpells = new();

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            foreach (var spell in _pooledSpells.Keys)
            {
                // Unregister Netcode spawn handlers
                NetworkManager.Singleton.PrefabHandler.RemoveHandler(spell);
            }
            _pooledSpells.Clear();
        }

        public void RegisterSpellsForPlayerWithMagicSchoolIndex(int magicSchoolIndex)
        {
            var mofaBaseRealityManager = (MofaBaseRealityManager)HoloKitApp.HoloKitApp.Instance.RealityManager;
            int count = 0;
            foreach (var spell in mofaBaseRealityManager.SpellList.List)
            {
                if (int.Parse(spell.MagicSchool.TokenId) == magicSchoolIndex)
                {
                    count++;
                    RegisterSpell(spell.gameObject, spell.SpellType == SpellType.Basic ? _basicSpellPrewarmCount : _secondarySpellPrewarmCount);
                }
                if (count == 2)
                {
                    return;
                }
            }
        }

        public void RegisterSpell(GameObject spell, int prewarmCount)
        {
            Queue<NetworkObject> spellQueue = new();
            if (!_pooledSpells.ContainsKey(spell))
                _pooledSpells.Add(spell, spellQueue);
            for (int i = 0; i < prewarmCount; i++)
            {
                var spellInstance = Instantiate(spell);
                ReturnSpell(spellInstance.GetComponent<NetworkObject>(), spell);
            }
            // Register Netcode spawn handlers
            NetworkManager.Singleton.PrefabHandler.AddHandler(spell, new PooledSpellInstanceHandler(spell, this));
        }

        public void ReturnSpell(NetworkObject networkObject, GameObject spell)
        {
            networkObject.gameObject.SetActive(false);
            _pooledSpells[spell].Enqueue(networkObject);
        }

        public NetworkObject GetSpell(GameObject spell, Vector3 position, Quaternion rotation)
        {
            var spellQueue = _pooledSpells[spell];
            if (spellQueue.Count == 0)
            {
                RegisterSpell(spell, 1);
            }
            var networkObject = spellQueue.Dequeue();
            networkObject.gameObject.SetActive(true);
            networkObject.transform.SetPositionAndRotation(position, rotation);
            return networkObject;
        }
    }

    class PooledSpellInstanceHandler: INetworkPrefabInstanceHandler
    {
        private readonly GameObject _spell;

        private readonly MofaSpellPool _pool;

        public PooledSpellInstanceHandler(GameObject spell, MofaSpellPool pool)
        {
            _spell = spell;
            _pool = pool;
        }

        public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            return _pool.GetSpell(_spell, position, rotation);
        }

        public void Destroy(NetworkObject networkObject)
        {
            _pool.ReturnSpell(networkObject, _spell);
        }
    }
}
