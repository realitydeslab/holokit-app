using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{
    [CreateAssetMenu(menuName = "Flexible UI Data")]
    public class FlexibleUIData : ScriptableObject
    {
        [Header("State Color")]
        public Color WhiteActiveColor;
        public Color WhiteInactiveColor;
        public Color DarkActiveColor;
        public Color DarkInactiveColor;

        [Header("Holokit Color")]
        public Color holoWhite = Color.white;
        public Color holoGrey1 = new Color(243f / 255f, 243f / 255f, 243f / 255f);
        public Color holoGrey2 = new Color(231f / 255f, 231f / 255f, 231f / 255f);
        public Color holoGrey3 = new Color(199f / 255f, 199f / 255f, 199f / 255f);
        public Color holoGrey4 = new Color(94f / 255f, 94f / 255f, 94f / 255f);
        public Color holoOrange = new Color(1, 91 / 255f, 26 / 255f);
        public Color holoBlack = Color.black;


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

        [Header("Media Sprites")]
        public Sprite YoutubeWhite;
        public Sprite YoutubeBlack;
        public Sprite TwitterWhite;
        public Sprite TwitterBlack;
        public Sprite DiscordWhite;
        public Sprite DiscordBlack;
        public Sprite MailWhite;
        public Sprite MailBlack;
        public Sprite AppleWhite;
        public Sprite AppleBlack;
        public Sprite InstagramWhite;
        public Sprite InstagramBlack;
        public Sprite TiktokWhite;
        public Sprite TiktokBlack;
        public Sprite FacebookWhite;
        public Sprite FacebookBlack;
        public Sprite LinkedinWhite;
        public Sprite linkedinBlack;
        public Sprite OpenseaWhite;
        public Sprite OpenseaBlack;
    }

}