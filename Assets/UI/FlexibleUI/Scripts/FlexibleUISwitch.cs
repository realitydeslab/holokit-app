using UnityEngine;
using UnityEngine.UI;


namespace Holoi.HoloKit.App.UI
{
    public class FlexibleUISwitch : FlexibleUI
    {

        public enum State
        {
            Close,
            Open,
        }

        [SerializeField] State _state;

        Image _image;


        protected override void OnSkinUI()
        {
            base.OnSkinUI();

            _image = GetComponent<Image>();

            switch (_state)
            {
                case State.Close:
                    _image.sprite = SkinData.SwitchClose;
                    break;
                case State.Open:
                    _image.sprite = SkinData.SwitchOpen;
                    break;
            }
        }
    }

}