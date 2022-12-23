using System;
using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;

namespace Holoi.Library.MOFABase
{
    public enum MofaTeam
    {
        // None for spectators
        None = 0,
        Blue = 1,
        Red = 2
    }

    public class MofaPlayer : HoloKitAppPlayer
    {
        /// <summary>
        /// Each MofaPlayer has a team.
        /// </summary>
        public NetworkVariable<MofaTeam> Team = new(MofaTeam.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// This is the index of the magic school selected by this player.
        /// </summary>
        public NetworkVariable<int> MagicSchoolIndex = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// Whether this player is ready to fight?
        /// </summary>
        public NetworkVariable<bool> Ready = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// The number of kill this player has made in the current round.
        /// </summary>
        public NetworkVariable<int> Kill = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// The number of death of the player in the current round.
        /// </summary>
        public NetworkVariable<int> Death = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// How many times does the player cast spells in this round?
        /// </summary>
        public NetworkVariable<int> CastCount = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// How many times does the player hit in this round?
        /// </summary>
        public NetworkVariable<int> HitCount = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// The distance the player has moved in this round. 
        /// </summary>
        public NetworkVariable<float> Distance = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// The calories the player has burned in this round.
        /// </summary>
        public NetworkVariable<float> Energy = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        /// <summary>
        /// The corresponding life shield of the player.
        /// </summary>
        public LifeShield LifeShield;

        /// <summary>
        /// The offset from the center eye point to the center of the life shield.
        /// </summary>
        public Vector3 CenterEyeToLifeShieldOffset = new(0f, -0.4f, 0.5f);

        public float AltDistance => _altDistance;

        /// <summary>
        /// This is the backup when the player doesn't have an Apple Watch to calculate
        /// the distance. This is calculated by accumulating the difference between two adjacent frames.
        /// </summary>
        private float _altDistance;

        /// <summary>
        /// This is used to calculate the total distance.
        /// </summary>
        private Vector3 _lastFramePosition;

        public static event Action<MofaPlayer> OnMofaPlayerReadyChanged;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Ready.OnValueChanged += OnReadyValueChanged;

            // The server allocates the team for each player
            if (IsServer)
            {
                if (IsLocalPlayer)
                    Team.Value = MofaTeam.Blue;
                else
                    Team.Value = MofaTeam.Red;
            }

            if (IsOwner)
                InitMofaPlayerInfoServerRpc(int.Parse(HoloKitApp.HoloKitApp.Instance.GlobalSettings.GetPreferencedObject().TokenId));
        }

        [ServerRpc]
        private void InitMofaPlayerInfoServerRpc(int magicSchoolIndex)
        {
            MagicSchoolIndex.Value = magicSchoolIndex;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            Ready.OnValueChanged -= OnReadyValueChanged;
        }

        protected virtual void Update()
        {
            if (IsServer)
            {
                // We only compute horizontal distance
                Vector3 horizontalPosition = Vector3.ProjectOnPlane(transform.position, Vector3.up);
                _altDistance += Vector3.Distance(_lastFramePosition, horizontalPosition);
                _lastFramePosition = horizontalPosition;
            }
        }

        [ServerRpc]
        public void SetReadyServerRpc(bool ready)
        {
            Ready.Value = ready;
        }

        private void OnReadyValueChanged(bool oldValue, bool newValue)
        {
            if (oldValue == newValue) return;

            OnMofaPlayerReadyChanged?.Invoke(this);
        }

        /// <summary>
        /// Reset the player's stats. This is usually called at the beginning of the round.
        /// </summary>
        public void ResetStats()
        {
            if (!IsServer)
            {
                Debug.LogError("[MofaPlayer] Player stats can only be set by the server");
                return;
            }
            Kill.Value = 0;
            Death.Value = 0;
            CastCount.Value = 0;
            HitCount.Value = 0;
            Distance.Value = 0f;
            _altDistance = 0f;
            Energy.Value = 0f;
        }

        [ServerRpc]
        public void UpdateHealthDataServerRpc(float distance, float energy)
        {
            Distance.Value = distance;
            Energy.Value = energy;
        }
    }
}