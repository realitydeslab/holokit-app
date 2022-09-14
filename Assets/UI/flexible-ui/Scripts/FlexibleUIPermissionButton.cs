using UnityEngine;
using UnityEngine.UI;


namespace Holoi.Library.HoloKitApp.UI
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class FlexibleUIPermissionButton : FlexibleUI
    {
        public enum GoOnState
        {
            Uncheck,
            Checked
        }

        //public Theme theme;
        public GoOnState state;

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

            //_button.transition = Selectable.Transition.SpriteSwap; // set transition mode
            _button.targetGraphic = _image;

            _image.sprite = SkinData.ButtonStrokeSprite;
            _image.type = Image.Type.Sliced;
            //_button.spriteState = SkinData.ButtonSpriteState;



            switch (state)
            {
                case GoOnState.Uncheck:
                    _image.color = SkinData.WhiteActiveColor;
                    _icon.sprite = SkinData.UnchenckCircle;
                    _icon.color = Color.black;
                    _text.color = Color.black;
                    _text.text = _string;

                    break;
                case GoOnState.Checked:
                    _image.color = SkinData.DarkActiveColor;
                    _icon.sprite = SkinData.CheckedCircle;
                    _icon.color = Color.white;
                    _text.color = Color.white;
                    _text.text = _string;
                    break;
            }
        
        }
    }

}