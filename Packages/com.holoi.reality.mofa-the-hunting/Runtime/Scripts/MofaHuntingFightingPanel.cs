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
                Scores.gameObject.SetActive(true);
                Time.gameObject.SetActive(true);
            }
            else if (HoloKitApp.Instance.IsMaster)
            {
                Reticle.gameObject.SetActive(true);
            }
            else
            {
                //Scores.SetActive(true);
                //Time.SetActive(true);
                Reticle.gameObject.SetActive(true);
                Status.gameObject.SetActive(true);
                RedScreen.gameObject.SetActive(true);
            }
        }
    }
}
