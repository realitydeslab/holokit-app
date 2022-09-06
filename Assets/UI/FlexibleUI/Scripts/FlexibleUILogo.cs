using UnityEngine;
using UnityEngine.UI;


namespace Holoi.HoloKit.App.UI
{
    public class FlexibleUILogo : FlexibleUI
    {

        public enum Style
        {
            Style,
            Style2
        }

        public enum Theme
        {
            Black,
            White
        }

        [SerializeField] Style _style;
        [SerializeField] Theme _color;

        Image _image;

        protected override void OnSkinUI()
        {
            base.OnSkinUI();

            _image = GetComponent<Image>();

            switch (_style)
            {
                case Style.Style:
                    transform.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 120);
                    switch (_color)
                    {
                        case Theme.Black:
                            _image.sprite = SkinData.LogoABlack;
                            break;
                        case Theme.White:
                            _image.sprite = SkinData.LogoAWhite;
                            break;
                    }
                    break;
                case Style.Style2:
                    transform.GetComponent<RectTransform>().sizeDelta = new Vector2(404.64f, 84);
                    switch (_color)
                    {
                        case Theme.Black:
                            _image.sprite = SkinData.LogoBBlack;

                            break;
                        case Theme.White:
                            _image.sprite = SkinData.LogoBWhite;
                            break;
                    }
                    break;
            }
        }
    }

}