using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Holoi.HoloKit.App.UI
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class FlexibleUIButton : FlexibleUI
    {
        public enum Theme
        {
            White,
            Black
        }

        public Theme theme;

        Image _image;
        Image _icon;
        [SerializeField]
        private string _string = "";
        TMPro.TMP_Text _text;
        Button _button;


        protected override void OnSkinUI()
        {
            base.OnSkinUI();

            _image = GetComponent<Image>();
            _icon = transform.Find("Icon").GetComponent<Image>();
            _text = transform.Find("Text").GetComponent<TMPro.TMP_Text>();
            _button = GetComponent<Button>();

            _button.transition = Selectable.Transition.SpriteSwap; // set transition mode
            _button.targetGraphic = _image;

            _image.sprite = SkinData.ButtonSprite;
            _image.type = Image.Type.Sliced;
            _button.spriteState = SkinData.ButtonSpriteState;


            switch (theme)
            {
                case Theme.Black:
                    _image.color = SkinData.DefaultColor;
                    _icon.sprite = SkinData.DefaultIcon;
                    _text.color = Color.black;
                    _text.text = _string;
                    break;
                case Theme.White:
                    _image.color = SkinData.ContractColor;
                    _icon.sprite = SkinData.ContractIcon;
                    _text.color = Color.white;
                    _text.text = _string;
                    break;
            }
        }
    }

}