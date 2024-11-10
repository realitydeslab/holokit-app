// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
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
