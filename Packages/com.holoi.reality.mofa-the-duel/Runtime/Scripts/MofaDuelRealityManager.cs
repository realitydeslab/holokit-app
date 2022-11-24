using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFATheDuel
{
    public class MofaDuelRealityManager : MofaBaseRealityManager
    {
        protected override void Start()
        {
            base.Start();
            MofaPlayer.OnMofaPlayerReadyStateChanged += OnPlayerReadyStateChanged;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            MofaPlayer.OnMofaPlayerReadyStateChanged -= OnPlayerReadyStateChanged;
        }

        public override void TryStartRound()
        {
            if (CurrentPhase == MofaPhase.Waiting || CurrentPhase == MofaPhase.RoundData)
            {
                GetPlayer().Ready.Value = true;
            }
        }

        public virtual void OnPlayerReadyStateChanged(ulong clientId, bool ready)
        {
            if (!IsServer) return;
            if (!ready) return;
            if (Players.Count < 2) return;

            foreach (var player in Players.Values)
            {
                if (!player.Ready.Value)
                {
                    return;
                }
            }
            StartCoroutine(StartRoundFlow());
        }
    }
}
