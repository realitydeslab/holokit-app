// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Holoi.Library.Permissions;

namespace Holoi.Library.HoloKitApp.UI
{
    public abstract class HoloKitAppUITemplate_PermissionsPage_PermissionButton : MonoBehaviour
    {
        [SerializeField] private Button _permissionButton;

        [SerializeField] private TMP_Text _permissionText;

        [SerializeField] private Image _checkmarkImage;

        [SerializeField] private Sprite _checkedSprite;

        [SerializeField] private Sprite _uncheckedSprite;

        private readonly Color _whiteColor = new(1f, 1f, 1f, 1f);

        private readonly Color _blackColor = new(0f, 0f, 0f, 1f);

        private void Start()
        {
            UpdatePermissionButton(GetPermissionStatus());
        }

        protected void UpdatePermissionButton(HoloKitAppPermissionStatus permissionStatus)
        {
            ColorBlock colorBlock = _permissionButton.colors;
            _permissionButton.onClick.RemoveAllListeners();
            switch (permissionStatus)
            {
                case HoloKitAppPermissionStatus.NotDetermined:
                    colorBlock.normalColor = _whiteColor;
                    colorBlock.selectedColor = _whiteColor;
                    _permissionText.color = _blackColor;
                    _checkmarkImage.sprite = _uncheckedSprite;
                    _permissionButton.onClick.AddListener(RequestPermission);
                    break;
                case HoloKitAppPermissionStatus.Denied:
                    colorBlock.normalColor = _whiteColor;
                    colorBlock.selectedColor = _whiteColor;
                    _permissionText.color = _blackColor;
                    _checkmarkImage.sprite = _uncheckedSprite;
                    _permissionButton.onClick.AddListener(() =>
                    {
                        PermissionsAPI.OpenAppSettings();
                    });
                    break;
                case HoloKitAppPermissionStatus.Granted:
                    colorBlock.normalColor = _blackColor;
                    colorBlock.selectedColor = _blackColor;
                    _permissionText.color = _whiteColor;
                    _checkmarkImage.sprite = _checkedSprite;
                    break;
            }
            _permissionButton.colors = colorBlock;
        }

        protected abstract HoloKitAppPermissionStatus GetPermissionStatus();

        protected abstract void RequestPermission();
    }
}
