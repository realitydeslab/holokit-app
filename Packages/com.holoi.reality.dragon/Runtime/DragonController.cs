// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
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
