// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

namespace Holoi.Library.MOFABase
{
    public interface IDamageable
    {
        public void OnDamaged(ulong attackerClientId);
    }
}