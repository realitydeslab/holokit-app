using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{

    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class FlexibleUIBackButton : FlexibleUI
    {
        public enum Theme
        {
            Dark,
            Bright
        }

        public Theme theme;

        Image _image;
        //Button _button;


        protected override void OnSkinUI()
        {
            base.OnSkinUI();

            _image = transform.Find("Image").GetComponent<Image>();

            //_image = GetComponent<Image>();
            //_button = GetComponent<Button>();

            switch (theme)
            {
                case Theme.Dark:
                    _image.sprite = SkinData.BackButtonSprite;
                    _image.color = SkinData.holoWhite;
                    break;
                case Theme.Bright:
                    _image.sprite = SkinData.BackButtonSprite;
                    _image.color = SkinData.holoBlack;
                    break;
            }
        }
    }
}