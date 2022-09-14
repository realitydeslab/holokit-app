using UnityEngine;
using UnityEngine.UI;


namespace Holoi.Library.HoloKitApp.UI
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class FlexibleUIExitButton : FlexibleUI
    {
        public enum Theme
        {
            Default,
            Contract
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
                case Theme.Default:
                    _image.sprite = SkinData.ExitButtonSprite;
                    _image.color = SkinData.WhiteActiveColor;
                    break;
                case Theme.Contract:
                    _image.sprite = SkinData.ExitButtonSprite;
                    _image.color = SkinData.DarkActiveColor;
                    break;
            }
        }
    }
}