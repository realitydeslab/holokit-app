// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.VFX;
using UnityEngine.Animations;

namespace Holoi.Reality.QuantumRealm
{
    public class NetworkHand : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            //var vfx = FindObjectOfType<VisualEffect>();
            var handPosition = GameObject.Find("Local Hand");
            var constraintSource = new ConstraintSource();
            constraintSource.sourceTransform = transform;
            constraintSource.weight = 1f;
            var parentConstraint = handPosition.GetComponent<ParentConstraint>();
            parentConstraint.AddSource(constraintSource);
            parentConstraint.constraintActive = true;
        }
    }
}
