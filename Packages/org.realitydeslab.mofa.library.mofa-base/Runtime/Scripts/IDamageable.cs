// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

namespace RealityDesignLab.MOFA.Library.MOFABase
{
    public interface IDamageable
    {
        public void OnDamaged(ulong attackerClientId);
    }
}