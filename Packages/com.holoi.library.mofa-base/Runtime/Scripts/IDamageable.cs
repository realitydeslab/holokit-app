// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

namespace Holoi.Library.MOFABase
{
    public interface IDamageable
    {
        public void OnDamaged(ulong attackerClientId);
    }
}