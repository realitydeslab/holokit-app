using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;

namespace Holoi.Library.HoloKitApp.UI
{

    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class FlexibleUITechButton : FlexibleUI
    {
        public RealityTag tag;
        TMPro.TMP_Text _text;

        protected override void OnSkinUI()
        {
            base.OnSkinUI();

            _text = GetComponent<FlexibleUIText>().text;

            switch (tag.id)
            {
                case "com.holoi.reality.tag.applehandtracking":
                    _text.text = "#" + "HandTracking";
                    break;
                case "com.holoi.reality.tag.applewatchasmotioncontroller":
                    _text.text = "#" + "AppleWatch";
                    break;
                case "com.holoi.reality.tag.arsessionbreakable":
                    _text.text = "#" + "SessionBrakable";
                    break;
                case "com.holoi.reality.tag.litehandtracking":
                    _text.text = "#" + "LiteHandTracking";
                    break;
                case "com.holoi.reality.tag.phaseexperience":
                    _text.text = "#" + "Phase Experience";
                    break;
                case "com.holoi.reality.tag.supportspectatorcontrol":
                    _text.text = "#" + "SpectatorControl";
                    break;
                case "com.holoi.reality.tag.supportspectatorview":
                    _text.text = "#" + "SpectatorView";
                    break;
                case "com.holoi.reality.tag.sampletag":
                    _text.text = "#" + "Sample tag";
                    break;
                case null:
                    _text.text = "#" + "not found tag";
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