using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{

    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class FlexibleUIBackButton : FlexibleUI
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
                    _image.sprite = SkinData.BackButtonSprite;
                    _image.color = SkinData.DefaultColor;
                    break;
                case Theme.Contract:
                    _image.sprite = SkinData.BackButtonSprite;
                    _image.color = SkinData.ContractColor;
                    break;
            }
        }
    }
}