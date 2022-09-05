using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{
    [CreateAssetMenu(menuName = "Flexible UI Data")]
    public class FlexibleUIData : ScriptableObject
    {
        [Header("Theme Color")]
        public Color DefaultColor;
        public Color ContractColor;

        [Header("Icon List")]
        public Sprite DefaultArrow;
        public Sprite ContractArrow;
        public Sprite UnchenckCircle;
        public Sprite CheckedCircle;

        [Header("ButtonSprites")]
        public Sprite ButtonSprite;
        public SpriteState ButtonSpriteState; // create 4 sprites on inspector


        [Header("BackButtonSprites")]
        public Sprite BackButtonSprite;
        [Header("ExitButtonSprites")]
        public Sprite ExitButtonSprite;
        [Header("StARButtonSprites")]
        public Sprite StARButtonSprite;
        [Header("SpectatorButtonSprites")]
        public Sprite SpectatorButtonSprite;



    }

}