using UnityEngine;
using UnityEngine.UI;


namespace Holoi.Library.HoloKitApp.UI
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
            loading =8
        }

        public enum Font
        {
            Normal = 0,
            Slanted = 1
        }

        [SerializeField] Type _type;
        [SerializeField] Font _font;

        TMPro.TMP_Text _text;


        protected override void OnSkinUI()
        {
            base.OnSkinUI();

            _text = transform.Find("Text").GetComponent<TMPro.TMP_Text>();
            _text.raycastTarget = true;

            switch (_type)
            {
                case Type.H1:
                    switch (_font)
                    {
                        case Font.Normal:
                            SetText(Color.black, SkinData.Bold, 96, -1.61f, 0, 16.67f);
                            break;
                        case Font.Slanted:
                            SetText(Color.black, SkinData.BoldSlanted, 96, -1.61f, 0, 16.67f);
                            break;
                    }
                    break;
                case Type.H2:
                    switch (_font)
                    {
                        case Font.Normal:
                            SetText(Color.black, SkinData.Bold, 66, 0f, 0, 15);
                            break;
                        case Font.Slanted:
                            SetText(Color.black, SkinData.BoldSlanted, 66, 0f, 0, 15);
                            break;
                    }
                    break;
                case Type.H3:
                    switch (_font)
                    {
                        case Font.Normal:
                            SetText(Color.black, SkinData.Bold, 41, 0.7f, 0, 0);
                            break;
                        case Font.Slanted:
                            SetText(Color.black, SkinData.BoldSlanted, 41, 0.7f, 0, 0);
                            break;
                    }
                    break;
                case Type.body1:
                    switch (_font)
                    {
                        case Font.Normal:
                            SetText(Color.black, SkinData.Regular, 48, -1.34f, 1.06f, 39.78f);
                            break;
                        case Font.Slanted:
                            SetText(Color.black, SkinData.RegularSlanted, 48, -1.34f, 1.06f, 39.78f);
                            break;
                    }
                    break;
                case Type.body2:
                    switch (_font)
                    {
                        case Font.Normal:
                            SetText(Color.black, SkinData.Regular, 42, -1.24f, 1.06f, 47.25f);
                            break;
                        case Font.Slanted:
                            SetText(Color.black, SkinData.RegularSlanted, 42, -1.24f, 1.06f, 47.25f);
                            break;
                    }
                    break;
                case Type.menu:
                    switch (_font)
                    {
                        case Font.Normal:
                            SetText(Color.black, SkinData.Bold, 74, 1.5f, 0, 0);
                            break;
                        case Font.Slanted:
                            SetText(Color.black, SkinData.BoldSlanted, 74, 1.5f, 0, 0);
                            break;
                    }
                    break;
                case Type.loading:
                    switch (_font)
                    {
                        case Font.Normal:
                            SetText(Color.black, SkinData.Bold, 88, .38f, 2.45f, 16.67f);
                            break;
                        case Font.Slanted:
                            SetText(Color.black, SkinData.BoldSlanted, 88, .38f, 2.45f, 16.67f);
                            break;
                    }
                    break;
                case Type.debug:
                    switch (_font)
                    {
                        case Font.Normal:
                            SetText(Color.black, SkinData.Thin, 30, 2.25f, 1.06f, 47.25f);
                            break;
                        case Font.Slanted:
                            SetText(Color.black, SkinData.ThinSlanted, 30, 2.25f, 1.06f, 47.25f);
                            break;
                    }
                    break;
            }
        }

        void SetText(Color color, TMPro.TMP_FontAsset font, float size, float cSpacing, float wSpacing, float lSpacing)
        {
            //_text.color = color;
            _text.font = font;
            _text.fontSize = size;
            _text.characterSpacing = cSpacing;
            _text.wordSpacing = wSpacing;
            _text.lineSpacing = lSpacing;
        }
    }

}