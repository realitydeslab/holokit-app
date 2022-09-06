using UnityEngine;
using UnityEngine.UI;


namespace Holoi.HoloKit.App.UI
{
    public class FlexibleUIText : FlexibleUI
    {

        public enum Type
        {
            H1,
            H2,
            H3,
            H3Slanted,
            body1,
            body2,
        }

        [SerializeField] Type _type;

        TMPro.TMP_Text _text;


        protected override void OnSkinUI()
        {
            base.OnSkinUI();

            _text = transform.Find("Text").GetComponent<TMPro.TMP_Text>();

            switch (_type)
            {
                case Type.H1:
                    SetText(Color.black, SkinData.Bold, 96, -1.61f ,0, 16.67f);
                    break;
                case Type.H2:
                    SetText(Color.black, SkinData.Bold, 66, 0f, 0, 15);
                    break;
                case Type.H3:
                    SetText(Color.black, SkinData.Bold, 41, 0.7f, 0, 0);
                    break;
                case Type.H3Slanted:
                    SetText(Color.black, SkinData.BoldSlanted, 62, 3.06f, 0, 0);
                    break;
                case Type.body1:
                    SetText(Color.black, SkinData.Regular, 48, -1.34f, 1.06f, 39.78f);
                    break;
                case Type.body2:
                    SetText(Color.black, SkinData.Regular, 42, -1.24f, 1.06f, 47.25f);

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