using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class FlexibleUIIconButton : FlexibleUI
    {
        public enum Icon
        {
            Youtube,
            Twitter,
            Discord,
            Mail,
            Apple,
            Instagram,
            Tiktok,
            Facebook,
            Linkedin,
            Opensea

        }
        public enum Theme
        {
            Dark,
            Bright
        }

        public Theme theme;
        public Icon _icon;
        Image _image;

        protected override void OnSkinUI()
        {
            base.OnSkinUI();

            _image = GetComponent<Image>();

            switch (_icon)
            {
                case Icon.Youtube:
                    switch (theme)
                    {
                        case Theme.Dark:
                            _image.sprite = SkinData.YoutubeWhite;
                            break;
                        case Theme.Bright:
                            _image.sprite = SkinData.YoutubeBlack;
                            break;
                    }
                    break;
                case Icon.Twitter:
                    switch (theme)
                    {
                        case Theme.Dark:
                            _image.sprite = SkinData.TwitterWhite;
                            break;
                        case Theme.Bright:
                            _image.sprite = SkinData.TwitterBlack;
                            break;
                    }
                    break;
                case Icon.Discord:
                    switch (theme)
                    {
                        case Theme.Dark:
                            _image.sprite = SkinData.DiscordWhite;
                            break;
                        case Theme.Bright:
                            _image.sprite = SkinData.DiscordBlack;
                            break;
                    }
                    break;
                case Icon.Mail:
                    switch (theme)
                    {
                        case Theme.Dark:
                            _image.sprite = SkinData.MailWhite;
                            break;
                        case Theme.Bright:
                            _image.sprite = SkinData.MailBlack;
                            break;
                    }
                    break;
                case Icon.Apple:
                    switch (theme)
                    {
                        case Theme.Dark:
                            _image.sprite = SkinData.AppleWhite;
                            break;
                        case Theme.Bright:
                            _image.sprite = SkinData.AppleBlack;
                            break;
                    }
                    break;
                case Icon.Instagram:
                    switch (theme)
                    {
                        case Theme.Dark:
                            _image.sprite = SkinData.InstagramWhite;
                            break;
                        case Theme.Bright:
                            _image.sprite = SkinData.InstagramBlack;
                            break;
                    }
                    break;
                case Icon.Tiktok:
                    switch (theme)
                    {
                        case Theme.Dark:
                            _image.sprite = SkinData.TiktokWhite;
                            break;
                        case Theme.Bright:
                            _image.sprite = SkinData.TiktokBlack;
                            break;
                    }
                    break;
                case Icon.Facebook:
                    switch (theme)
                    {
                        case Theme.Dark:
                            _image.sprite = SkinData.FacebookWhite;
                            break;
                        case Theme.Bright:
                            _image.sprite = SkinData.FacebookBlack;
                            break;
                    }
                    break;
                case Icon.Linkedin:
                    switch (theme)
                    {
                        case Theme.Dark:
                            _image.sprite = SkinData.LinkedinWhite;
                            break;
                        case Theme.Bright:
                            _image.sprite = SkinData.linkedinBlack;
                            break;
                    }
                    break;
                case Icon.Opensea:
                    switch (theme)
                    {
                        case Theme.Dark:
                            _image.sprite = SkinData.OpenseaWhite;
                            break;
                        case Theme.Bright:
                            _image.sprite = SkinData.OpenseaBlack;
                            break;
                    }
                    break;
            }

        }
    }

}