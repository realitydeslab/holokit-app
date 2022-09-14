using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{

    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class FlexibleUITechButton : FlexibleUI
    {
        public enum Type
        {
            SpatialAudio = 0,
            MotionConttrol = 1,
            AppleWatch = 2,
            HandTracking = 3,
            SpectatorView = 4,
            Multiplayer = 5,
            LiDARRequired = 6,
            StAROnly = 7

        }

        public Type type;
        TMPro.TMP_Text _text;


        protected override void OnSkinUI()
        {
            base.OnSkinUI();

            _text = GetComponent<FlexibleUIText>().text;

            switch (type)
            {
                case Type.SpatialAudio:
                    _text.text = "#" + "SpatialAudio";
                    break;
                case Type.MotionConttrol:
                    _text.text = "#" + "MotionConttrol";
                    break;
                case Type.AppleWatch:
                    _text.text = "#" + "AppleWatch";
                    break;
                case Type.HandTracking:
                    _text.text = "#" + "HandTracking";
                    break;
                case Type.SpectatorView:
                    _text.text = "#" + "SpectatorView";
                    break;
                case Type.Multiplayer:
                    _text.text = "#" + "Multiplayer";
                    break;
                case Type.LiDARRequired:
                    _text.text = "#" + "LiDARRequired";
                    break;
                case Type.StAROnly:
                    _text.text = "#" + "StAROnly";
                    break;
            }

            GetComponent<RectTransform>().sizeDelta = new Vector2(
                GetComponent<FlexibleUIText>().text.preferredWidth + 72,
                69);
            GetComponent<LayoutElement>().preferredWidth = GetComponent<FlexibleUIText>().text.preferredWidth + 72;
            GetComponent<LayoutElement>().preferredHeight = 69;
        }
    }
}