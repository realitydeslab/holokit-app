using UnityEngine;
using UnityEngine.UI;


namespace Holoi.HoloKit.App.UI
{
    public class FlexibleUIPlayButton : FlexibleUI
    {
        public enum Theme
        {
            Dark,
            Bright
        }

        public Theme theme;
        Image _image;

        protected override void OnSkinUI()
        {
            base.OnSkinUI();

            _image = GetComponent<Image>();

            switch (theme)
            {
                case Theme.Dark:
                    _image.sprite = SkinData.PlayWhite;
                    break;
                case Theme.Bright:
                    _image.sprite = SkinData.PlayBlack;
                    break;
            }
        }
    }

}