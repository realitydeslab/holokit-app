// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Apple.PHASE;

public class DragonController : MonoBehaviour
{
    public void PlaySound(AnimationEvent animationEvent)
    {
        GetComponent<AudioSource>().Play();
        PHASESource ps = GetComponent<PHASESource>();
        ps.Play();
    }
}
