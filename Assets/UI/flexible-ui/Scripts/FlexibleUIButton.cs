using UnityEngine;
using UnityEngine.UI;


namespace Holoi.HoloKit.App.UI
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
            Default,
            Banner
        }


        public enum State
        {
            Inactive,
            Active
        }

        public Color color = Color.Black;
        public Size size = Size.Default;
        public State state = State.Active;

        Image _image;
        Image _icon;
        [SerializeField]
        private string _string = "";
        TMPro.TMP_Text _text;
        Button _button;

        Vector2 _sizeDefault = new Vector2(978, 207);
        Vector2 _sizeHamburger = new Vector2(978, 129);

        protected override void OnSkinUI()
        {
            base.OnSkinUI();

            switch (size)
            {
                case Size.Default:
                    GetComponent<RectTransform>().sizeDelta = _sizeDefault;
                    break;
                case Size.Banner:
                    GetComponent<RectTransform>().sizeDelta = _sizeHamburger;
                    break;
            }
            _image = GetComponent<Image>();
            _icon = transform.Find("Icon").GetComponent<Image>();
            _text = transform.Find("Text").GetComponent<TMPro.TMP_Text>();
            _text.font = SkinData.BoldSlanted;
            _text.characterSpacing = -1.94f;
            _button = GetComponent<Button>();

            //_button.transition = Selectable.Transition.SpriteSwap; // set transition mode
            _button.targetGraphic = _image;

            _image.type = Image.Type.Sliced;
            //_button.spriteState = SkinData.ButtonSpriteState;


            switch (color)
            {
                case Color.Black:
                    _image.sprite = SkinData.ButtonSprite;  
                    switch (state)
                    {
                        case State.Inactive:
                            _image.color = SkinData.DarkInactiveColor;
                            _icon.sprite = SkinData.WhiteArrow;
                            _text.color = UnityEngine.Color.white;
                            _text.text = _string;
                            break;
                        case State.Active:
                            _image.color = SkinData.DarkActiveColor;
                            _icon.sprite = SkinData.WhiteArrow;
                            _text.color = UnityEngine.Color.white;
                            _text.text = _string;
                            break;
                    }
                    break;
                case Color.White:
                    _image.sprite = SkinData.ButtonStrokeSprite;

                    switch (state)
                    {
                        case State.Inactive:
                            _image.color = SkinData.WhiteInactiveColor;
                            _icon.sprite = SkinData.BlackArrow;
                            _text.color = UnityEngine.Color.black;
                            _text.text = _string;
                            break;
                        case State.Active:
                            _image.color = SkinData.WhiteActiveColor;
                            _icon.sprite = SkinData.BlackArrow;
                            _text.color = UnityEngine.Color.black;
                            _text.text = _string;
                            break;
                    }
                    break;
            }
        }
    }

}