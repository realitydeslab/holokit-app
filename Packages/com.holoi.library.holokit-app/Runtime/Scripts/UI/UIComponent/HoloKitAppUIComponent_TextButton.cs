// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Holoi.Library.HoloKitApp.UI
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
