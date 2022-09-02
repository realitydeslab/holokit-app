using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Flexible UI Data")]
public class FlexibleUIData : ScriptableObject
{
    public Sprite ButtonSprite;
    public SpriteState ButtonSpriteState;

    public Color DefaultColor;
    public Sprite DefaultIcon;

    public Color ContractColor;
    public Sprite ContractIcon;

    public Color ComfirmColor;
    public Sprite ComfirmIcon;

    public Color DeclineColor;
    public Sprite DeclineIcon;

    public Color WarningColor;
    public Sprite WarningIcon;
}
