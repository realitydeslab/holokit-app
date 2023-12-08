// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.Reality.Typed.TheScanner
{
    public class TheScannerCenter : MonoBehaviour
    {
        public static TheScannerCenter Instance { get { return _instance; } }

        private static TheScannerCenter _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                _instance = this;
            }
        }
    }
}
