using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFATheDuel
{
    public class MofaDuelRealityManager : MofaBaseRealityManager
    {
        private void Start()
        {
            
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            
        }

        public override void StartRound()
        {
            //if (CurrentPhase == MofaPhase.Waiting || CurrentPhase == MofaPhase.RoundData)
            //{
            //    GetPlayer().Ready.Value = true;
            //}
        }

        public virtual void OnPlayerReadyStateChanged(ulong clientId, bool ready)
        {
            //if (!IsServer) return;
            //if (!ready) return;
            //if (PlayerDict.Count < 2) return;

            //foreach (var player in PlayerDict.Values)
            //{
            //    if (!player.Ready.Value)
            //    {
            //        return;
            //    }
            //}
            //StartCoroutine(StartBaseRoundFlow());
        }
    }
}
