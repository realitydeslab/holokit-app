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

            _text.text = "#" + tag.DisplayName;

            GetComponent<RectTransform>().sizeDelta = new Vector2(
                GetComponent<FlexibleUIText>().text.preferredWidth + 72,
                69);
            GetComponent<LayoutElement>().preferredWidth = GetComponent<FlexibleUIText>().text.preferredWidth + 72;
            GetComponent<LayoutElement>().preferredHeight = 69;
        }
    }
}