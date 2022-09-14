using UnityEngine;
using UnityEngine.UI;


namespace Holoi.Library.HoloKitApp.UI
{
    public class FlexibleUIPlayButton : FlexibleUI
    {
        public enum Color
        {
            Black,
            White
        }

        [SerializeField] Color _color;
        Image _image;

        protected override void OnSkinUI()
        {
            base.OnSkinUI();

            _image = GetComponent<Image>();

            switch (_color)
            {
                case Color.Black:
                    _image.sprite = SkinData.PlayBlack;
                    break;
                case Color.White:
                    _image.sprite = SkinData.PlayWhite;
                    break;
            }
        }
    }

}