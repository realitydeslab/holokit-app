// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using UnityEngine;

namespace Holoi.Library.HoloKitApp
{
    public class HoloKitAppPlayerPoseVisualizer : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text _distanceText;

        /// <summary>
        /// The offset from the player pose to the pose visualizer center.
        /// </summary>
        public Vector3 Offset = new(0f, 0.2f, 0f);

        /// <summary>
        /// The corresponding player of this pose visualizer. The pose visualizer should
        /// always follow the corresponding player.
        /// </summary>
        public HoloKitAppPlayer Player;

        private void LateUpdate()
        {
            // The pose visualizer should always look at the local camera.
            transform.LookAt(HoloKit.HoloKitCameraManager.Instance.CenterEyePose);
            if (Player != null)
            {
                transform.position = Player.transform.position + Offset;
                UpdateDistance();
            }
        }

        public void UpdateDistance()
        {
            _distanceText.text = Player.GetDistanceToLocalPlayer().ToString("F1") + "M";
        }
    }
}