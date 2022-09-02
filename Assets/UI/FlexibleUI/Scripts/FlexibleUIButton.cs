using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class FlexibleUIButton : FlexibleUI
{
    public enum ButtonType
    {
        Default,
        Contract,
        Comfirm,
        Decline,
        Warning
    }

    public ButtonType buttonType;

    Image _image;
    Image _icon;
    [SerializeField] string _string = "Connect To Ar World";
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


        switch (buttonType)
        {
            case ButtonType.Default:
                _image.color = SkinData.DefaultColor;
                _icon.sprite = SkinData.DefaultIcon;
                _text.color = Color.black;
                _text.text = _string;
                break;
            case ButtonType.Contract:
                _image.color = SkinData.ContractColor;
                _icon.sprite = SkinData.ContractIcon;
                _text.color = Color.white;
                _text.text = _string;
                break;
            case ButtonType.Comfirm:
                _image.color = SkinData.ComfirmColor;
                _icon.sprite = SkinData.ComfirmIcon;
                _text.text = _string;
                break;
            case ButtonType.Decline:
                _image.color = SkinData.DeclineColor;
                _icon.sprite = SkinData.DeclineIcon;
                _text.text = _string;
                break;
            case ButtonType.Warning:
                _image.color = SkinData.WarningColor;
                _icon.sprite = SkinData.WarningIcon;
                _text.text = _string;
                break;

        }
    }
}
