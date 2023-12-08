// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.Library.HoloKitApp.UI
{
    public class HoloKitAppUIComponent_ArrowChain : MonoBehaviour
    {
        [SerializeField] private GameObject _arrowChainElementPrefab;

        [SerializeField] private int _arrowCount;

        [SerializeField] private float _arrowInterval;

        [SerializeField] private float _arrowZPos;

        [SerializeField] private float _arrowScale;

        public float Length
        {
            get
            {
                return _arrowInterval * _arrowCount;
            }
        }

        private void Start()
        {
            float xPos = 0f;
            for (int i = 0; i < _arrowCount; i++)
            {
                var arrow = Instantiate(_arrowChainElementPrefab);
                arrow.transform.SetParent(transform);
                arrow.transform.localPosition = new(xPos, 0f, _arrowZPos);
                arrow.transform.localRotation = Quaternion.identity;
                arrow.transform.localScale = new(_arrowScale, _arrowScale, _arrowScale);
                xPos += _arrowInterval;
            }
        }
    }
}
