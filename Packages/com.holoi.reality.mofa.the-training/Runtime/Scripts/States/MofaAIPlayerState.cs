// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
// SPDX-License-Identifier: MIT

namespace Holoi.Reality.MOFA.TheTraining
{
    public abstract class MofaAIPlayerState
    {
        public abstract void OnEnter(MofaAIPlayer player);

        public abstract void OnUpdate(MofaAIPlayer player);

        public abstract void OnExit(MofaAIPlayer player);
    }
}
