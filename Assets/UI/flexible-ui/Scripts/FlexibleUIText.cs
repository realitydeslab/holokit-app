using UnityEngine;
using UnityEngine.UI;


namespace Holoi.HoloKit.App.UI
{
    public class FlexibleUIText : FlexibleUI
    {

        public enum Type
        {
            H1 = 0,
            H2 = 1,
            H3 = 3,
            body1 = 4,
            body2 = 5,
            debug = 6,
            menu = 7,
            loading = 8,
            techTag = 9
        }

        public enum Font
        {
            Normal = 0,
            Slanted = 1
        }

        public enum HolokitColor
        {
            White = 0,
            Grey1 = 1,
            Grey2 = 2,
            Grey3 = 3,
            Grey4 = 4,
            Black = 5,
            Orange = 6
        }


        public Type type;
        public Font font;
        public HolokitColor color;

        [HideInInspector] public TMPro.TMP_Text text;

        protected override void OnSkinUI()
        {
            base.OnSkinUI();

            text = transform.Find("Text").GetComponent<TMPro.TMP_Text>();
            //_text.raycastTarget = true;

            switch (type)
            {
                case Type.H1:
                    switch (font)
                    {
                        case Font.Normal:
                            SetText(color, SkinData.Bold, 96, -1.61f, 0, 16.67f);
                            break;
                        case Font.Slanted:
                            SetText(color, SkinData.BoldSlanted, 96, -1.61f, 0, 16.67f);
                            break;
                    }
                    break;
                case Type.H2:
                    switch (font)
                    {
                        case Font.Normal:
                            SetText(color, SkinData.Bold, 66, 0f, 0, 15);
                            break;
                        case Font.Slanted:
                            SetText(color, SkinData.BoldSlanted, 66, 0f, 0, 15);
                            break;
                    }
                    break;
                case Type.H3:
                    switch (font)
                    {
                        case Font.Normal:
                            SetText(color, SkinData.Bold, 41, 0.7f, 0, 0);
                            break;
                        case Font.Slanted:
                            SetText(color, SkinData.BoldSlanted, 41, 0.7f, 0, 0);
                            break;
                    }
                    break;
                case Type.body1:
                    switch (font)
                    {
                        case Font.Normal:
                            SetText(color, SkinData.Regular, 48, -1.34f, 1.06f, 39.78f);
                            break;
                        case Font.Slanted:
                            SetText(color, SkinData.RegularSlanted, 48, -1.34f, 1.06f, 39.78f);
                            break;
                    }
                    break;
                case Type.body2:
                    switch (font)
                    {
                        case Font.Normal:
                            SetText(color, SkinData.Regular, 42, -1.24f, 1.06f, 47.25f);
                            break;
                        case Font.Slanted:
                            SetText(color, SkinData.RegularSlanted, 42, -1.24f, 1.06f, 47.25f);
                            break;
                    }
                    break;
                case Type.menu:
                    switch (font)
                    {
                        case Font.Normal:
                            SetText(color, SkinData.Bold, 74, 1.5f, 0, 0);
                            break;
                        case Font.Slanted:
                            SetText(color, SkinData.BoldSlanted, 74, 1.5f, 0, 0);
                            break;
                    }
                    break;
                case Type.loading:
                    switch (font)
                    {
                        case Font.Normal:
                            SetText(color, SkinData.Bold, 88, .38f, 2.45f, 16.67f);
                            break;
                        case Font.Slanted:
                            SetText(color, SkinData.BoldSlanted, 88, .38f, 2.45f, 16.67f);
                            break;
                    }
                    break;
                case Type.techTag:
                    switch (font)
                    {
                        case Font.Normal:
                            SetText(color, SkinData.Bold, 30, 0f, 0f, 0f);
                            break;
                        case Font.Slanted:
                            SetText(color, SkinData.BoldSlanted, 30, 0f, 0f, 0f);
                            break;
                    }
                    break;
                case Type.debug:
                    switch (font)
                    {
                        case Font.Normal:
                            SetText(color, SkinData.Thin, 30, 2.25f, 1.06f, 47.25f);
                            break;
                        case Font.Slanted:
                            SetText(color, SkinData.ThinSlanted, 30, 2.25f, 1.06f, 47.25f);
                            break;
                    }
                    break;
            }
        }

        void SetText(HolokitColor hColor, TMPro.TMP_FontAsset font, float size, float cSpacing, float wSpacing, float lSpacing)
        {
            text.margin = new Vector4(0, 8.91f, 0, 0);
            text.color = HolokitColor2Color(hColor);
            text.font = font;
            text.fontSize = size;
            text.characterSpacing = cSpacing;
            text.wordSpacing = wSpacing;
            text.lineSpacing = lSpacing;
        }

        Color HolokitColor2Color(HolokitColor hColor)
        {
            switch (color)
            {
                case HolokitColor.White:
                    return SkinData.holoWhite;
                    break;
                case HolokitColor.Grey1:
                    return SkinData.holoGrey1;
                    break;
                case HolokitColor.Grey2:
                    return SkinData.holoGrey2;
                    break;
                case HolokitColor.Grey3:
                    return SkinData.holoGrey3;
                    break;
                case HolokitColor.Grey4:
                    return SkinData.holoGrey4;
                    break;
                case HolokitColor.Black:
                    return SkinData.holoBlack;
                    break;
                case HolokitColor.Orange:
                    return SkinData.holoOrange;
                    break;
            }
            return Color.white;
        }
    }

}