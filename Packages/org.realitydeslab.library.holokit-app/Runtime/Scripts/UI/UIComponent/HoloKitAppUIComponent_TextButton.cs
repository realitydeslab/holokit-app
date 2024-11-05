// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace RealityDesignLab.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_TextButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private TMP_Text _text;

        [SerializeField] private Color _normalColor;

        [SerializeField] private Color _pressedColor;

        private void Start()
        {
            _text.color = _normalColor;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            _text.color = _pressedColor;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            _text.color = _normalColor;
        }
    }
}
