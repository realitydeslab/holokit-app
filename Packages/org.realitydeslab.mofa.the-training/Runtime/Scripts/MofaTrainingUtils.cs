// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealityDesignLab.MOFA.TheTraining
{
    public static class MofaTrainingUtils
    {
        public static float InverseClamp(float value, float min, float max)
        {
            if (value < max && value > min)
            {
                return 0f;
            }
            else
            {
                return value;
            }
        }

        public static float Remap(float input, float in_min, float in_max, float out_min, float out_max, bool isClamp)
        {
            if (isClamp)
            {
                if (input > in_max) input = in_max;
                if (input < in_min) input = in_min;
            }
            else
            {

            }
            float o = ((input - in_min) / (in_max - in_min)) * (out_max - out_min) + out_min;
            return o;
        }
    }
}
