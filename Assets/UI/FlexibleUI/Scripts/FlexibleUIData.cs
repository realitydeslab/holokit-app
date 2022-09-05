using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{
    [CreateAssetMenu(menuName = "Flexible UI Data")]
    public class FlexibleUIData : ScriptableObject
    {
        [Header("LongButtonSprites")]
        public Sprite ButtonSprite;
        public SpriteState ButtonSpriteState; // create 4 sprites on inspector

        [Header("ButtonSprites")]
        public Sprite BackButtonSprite;
        public Sprite ExitButtonSprite;
        public Sprite StARButtonSprite;
        public Sprite SpectatorButtonSprite;


        [Header("Theme")]
        public Color DefaultColor;
        public Sprite DefaultIcon;

        public Color ContractColor;
        public Sprite ContractIcon;
    }

}