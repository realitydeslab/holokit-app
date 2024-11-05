// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealityDesignLab.Typed.TheMimic
{
    public class MimicHandController : MonoBehaviour
    {
        [SerializeField] Transform wrist;
        [SerializeField] Transform thumb;
        [SerializeField] Transform index;
        [SerializeField] Transform middle;
        [SerializeField] Transform ring;
        [SerializeField] Transform pinky;


        [SerializeField] Transform mwrist;
        [SerializeField] Transform mthumb;
        [SerializeField] Transform mindex;
        [SerializeField] Transform mmiddle;
        [SerializeField] Transform mring;
        [SerializeField] Transform mpinky;
        [SerializeField] float scale;

        Vector3 fixer = new Vector3(-1, 1, -1);

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            var relativeOffset = thumb.position - wrist.position;
            mthumb.position = mwrist.position + Vector3.Scale((relativeOffset * scale), fixer);

            relativeOffset = index.position - wrist.position;
            mindex.position = mwrist.position + Vector3.Scale((relativeOffset * scale), fixer);

            relativeOffset = middle.position - wrist.position;
            mmiddle.position = mwrist.position + Vector3.Scale((relativeOffset * scale), fixer);

            relativeOffset = ring.position - wrist.position;
            mring.position = mwrist.position + Vector3.Scale((relativeOffset * scale), fixer);

            relativeOffset = pinky.position - wrist.position;
            mpinky.position = mwrist.position + Vector3.Scale((relativeOffset * scale), fixer);
        }
    }
}
