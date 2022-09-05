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
        public enum State
        {
            Inactive,
            Active
        }

        public Theme theme;
        public State state;

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
            _text.font = SkinData.BoldSlanted;
            _text.characterSpacing = -1.94f;
            _button = GetComponent<Button>();

            _button.transition = Selectable.Transition.SpriteSwap; // set transition mode
            _button.targetGraphic = _image;

            _image.type = Image.Type.Sliced;
            _button.spriteState = SkinData.ButtonSpriteState;


            switch (theme)
            {
                case Theme.Black:
                    _image.sprite = SkinData.ButtonSprite;  
                    switch (state)
                    {
                        case State.Inactive:
                            _image.color = SkinData.DarkInactiveColor;
                            _icon.sprite = SkinData.WhiteArrow;
                            _text.color = Color.white;
                            _text.text = _string;
                            break;
                        case State.Active:
                            _image.color = SkinData.DarkActiveColor;
                            _icon.sprite = SkinData.WhiteArrow;
                            _text.color = Color.white;
                            _text.text = _string;
                            break;
                    }
                    break;
                case Theme.White:
                    _image.sprite = SkinData.ButtonStrokeSprite;

                    switch (state)
                    {
                        case State.Inactive:
                            _image.color = SkinData.WhiteInactiveColor;
                            _icon.sprite = SkinData.BlackArrow;
                            _text.color = Color.black;
                            _text.text = _string;
                            break;
                        case State.Active:
                            _image.color = SkinData.WhiteActiveColor;
                            _icon.sprite = SkinData.BlackArrow;
                            _text.color = Color.black;
                            _text.text = _string;
                            break;
                    }
                    break;
            }
        }
    }

}