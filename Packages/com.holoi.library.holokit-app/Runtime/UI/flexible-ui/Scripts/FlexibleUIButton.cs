using UnityEngine;
using UnityEngine.UI;


namespace Holoi.Library.HoloKitApp.UI
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class FlexibleUIButton : FlexibleUI
    {
        public enum Color
        {
            White,
            Black
        }
        public enum Size
        {
            Bold,
            Thin,
            Regular
        }


        public enum State
        {
            Active = 0,
            Inactive = 1
        }

        public Color color = Color.Black;
        public Size size = Size.Bold;
        public State state = State.Active;

        Image _image;
        Image _icon;
        [SerializeField]
        private string _string = "";
        TMPro.TMP_Text _text;
        Button _button;

        Vector2 _sizeBold = new Vector2(1002, 207);
        Vector2 _sizeThin = new Vector2(1002, 129);
        Vector2 _sizeRegular = new Vector2(1002, 168);

        protected override void OnSkinUI()
        {
            base.OnSkinUI();
            SetUIAppearance(state);
        }


        public void SetUIAppearance(State _state)
        {
            state = _state;

            switch (size)
            {
                case Size.Bold:
                    GetComponent<RectTransform>().sizeDelta = _sizeBold;
                    break;
                case Size.Regular:
                    GetComponent<RectTransform>().sizeDelta = _sizeRegular;
                    break;
                case Size.Thin:
                    GetComponent<RectTransform>().sizeDelta = _sizeThin;
                    break;
            }
            _image = GetComponent<Image>();
            _image.color = UnityEngine.Color.white;
            _icon = transform.Find("Icon").GetComponent<Image>();

            if (transform.Find("Text") == null)
            {

            }
            else
            {
                _text = transform.Find("Text").GetComponent<TMPro.TMP_Text>();
                _text.font = SkinData.BoldSlanted;
                _text.characterSpacing = -1.94f;
            }


            _button = GetComponent<Button>();
            _button.transition = Selectable.Transition.ColorTint; // set transition mode

            var block = new ColorBlock();
            block.normalColor = SkinData.holoBlack;
            block.pressedColor = SkinData.holoBlack;
            block.highlightedColor = SkinData.holoBlack;
            block.pressedColor = SkinData.holoBlack;
            block.selectedColor = SkinData.holoBlack;
            block.disabledColor = SkinData.holoBlack;
            block.colorMultiplier = 1;

            _button.targetGraphic = _image;
            //_button.spriteState = SkinData.ButtonSpriteState;

            _image.type = Image.Type.Sliced;



            switch (color)
            {
                case Color.Black:
                    _image.sprite = SkinData.ButtonSprite;
                    switch (state)
                    {
                        case State.Inactive:
                            block.normalColor = SkinData.holoGrey3;
                            block.selectedColor = SkinData.holoGrey3;
                            block.pressedColor = SkinData.holoGrey3;
                            _button.colors = block;
                            _icon.sprite = SkinData.WhiteArrow;
                            if (_text) _text.color = UnityEngine.Color.white;
                            if (_text) _text.text = _string;
                            break;
                        case State.Active:
                            block.normalColor = SkinData.holoBlack;
                            block.selectedColor = SkinData.holoBlack;
                            block.pressedColor = SkinData.holoGrey4;
                            _button.colors = block;
                            _icon.sprite = SkinData.WhiteArrow;
                            if (_text) _text.color = UnityEngine.Color.white;
                            if (_text) _text.text = _string;
                            break;
                    }
                    break;
                case Color.White:
                    _image.sprite = SkinData.ButtonStrokeSprite;

                    switch (state)
                    {
                        case State.Inactive:
                            block.normalColor = SkinData.WhiteInactiveColor;
                            block.selectedColor = SkinData.WhiteInactiveColor;
                            block.normalColor = SkinData.WhiteInactiveColor;
                            _button.colors = block;
                            _icon.sprite = SkinData.BlackArrow;
                            if (_text) _text.color = UnityEngine.Color.black;
                            if (_text) _text.text = _string;
                            break;
                        case State.Active:
                            block.normalColor = SkinData.holoWhite;
                            block.selectedColor = SkinData.holoWhite;
                            block.pressedColor = SkinData.holoGrey1;
                            _button.colors = block;
                            _icon.sprite = SkinData.BlackArrow;
                            if (_text) _text.color = UnityEngine.Color.black;
                            if (_text) _text.text = _string;
                            break;
                    }
                    break;
            }
        }
    }

}