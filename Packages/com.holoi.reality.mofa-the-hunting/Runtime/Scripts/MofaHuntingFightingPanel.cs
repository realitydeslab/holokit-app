using Holoi.Library.MOFABase;
using Holoi.Library.HoloKitApp;

namespace Holoi.Reality.MOFATheHunting
{
    public class MofaHuntingFightingPanel : MofaFightingPanel
    {
        protected override void OnCountdown()
        {
            if (HoloKitApp.Instance.IsSpectator) // Spectator
            {
                Scores.SetActive(true);
                Time.SetActive(true);
            }
            else if (HoloKitApp.Instance.IsHost)
            {
                Reticle.SetActive(true);
            }
            else // Not spectator
            {
                Scores.SetActive(true);
                Time.SetActive(true);
                Reticle.SetActive(true);
                Status.SetActive(true);
                RedScreen.SetActive(true);
            }
        }
    }
}
