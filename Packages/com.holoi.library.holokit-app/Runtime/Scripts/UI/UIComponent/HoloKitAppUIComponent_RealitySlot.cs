using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_RealitySlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text _realityId;

        [SerializeField] private TMP_Text _realityName;

        [SerializeField] private Button _hostButton;

        [SerializeField] private Button _playerButton;

        [SerializeField] private Button _puppeteerButton;

        [SerializeField] private Button _spectatorButton;

        public void Init(Reality reality, int index)
        {
            _realityId.text = "Reality #" + HoloKitAppUtils.IntToStringF3(index);
            _realityName.text = reality.DisplayName;
            _hostButton.onClick.AddListener(() =>
            {
                HoloKitApp.Instance.CurrentReality = reality;
                HoloKitApp.Instance.EnterRealityAs(HoloKitAppPlayerType.Host);
            });

            if (reality.IsMultiplayerSupported())
            {
                _playerButton.onClick.AddListener(() =>
                {
                    HoloKitApp.Instance.CurrentReality = reality;
                    HoloKitApp.Instance.EnterRealityAs(HoloKitAppPlayerType.NonHostPlayer);
                });
            }
            else
            {
                _playerButton.interactable = false;
            }

            if (reality.IsPuppeteerSupported())
            {
                _puppeteerButton.onClick.AddListener(() =>
                {
                    HoloKitApp.Instance.CurrentReality = reality;
                    HoloKitApp.Instance.EnterRealityAs(HoloKitAppPlayerType.Puppeteer);
                });
            }
            else
            {
                _puppeteerButton.interactable = false;
            }

            if (reality.IsSpectatorViewSupported())
            {
                _spectatorButton.onClick.AddListener(() =>
                {
                    HoloKitApp.Instance.CurrentReality = reality;
                    HoloKitApp.Instance.EnterRealityAs(HoloKitAppPlayerType.Spectator);
                });
            }
            else
            {
                _spectatorButton.interactable = false;
            }
        }
    }
}
