using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{
    [CreateAssetMenu(menuName = "Flexible UI Data")]
    public class FlexibleUIData : ScriptableObject
    {
        [Header("Theme Color")]
        public Color WhiteActiveColor;
        public Color WhiteInactiveColor;
        public Color DarkActiveColor;
        public Color DarkInactiveColor;

        [Header("Icon List")]
        public Sprite WhiteArrow;
        public Sprite BlackArrow;
        public Sprite UnchenckCircle;
        public Sprite CheckedCircle;

        [Header("ButtonSprites")]
        public Sprite ButtonSprite;
        public Sprite ButtonStrokeSprite;
        public SpriteState ButtonSpriteState; // create 4 sprites on inspector

        public Sprite BackButtonSprite;
        public Sprite ExitButtonSprite;
        public Sprite StARButtonSprite;
        public Sprite SpectatorButtonSprite;

        [Header("Font Assets")]
        public TMPro.TMP_FontAsset Heavy;
        public TMPro.TMP_FontAsset HeavySlanted;
        public TMPro.TMP_FontAsset Bold;
        public TMPro.TMP_FontAsset BoldSlanted;
        public TMPro.TMP_FontAsset Regular;
        public TMPro.TMP_FontAsset RegularSlanted;
        public TMPro.TMP_FontAsset Thin;
        public TMPro.TMP_FontAsset ThinSlanted;

        [Header("Logo Sprites")]
        public Sprite LogoAWhite;
        public Sprite LogoABlack;
        public Sprite LogoBWhite;
        public Sprite LogoBBlack;

        [Header("Switch Sprites")]
        public Sprite SwitchOpen;
        public Sprite SwitchClose;

        [Header("Play Sprites")]
        public Sprite PlayWhite;
        public Sprite PlayBlack;
    }

}