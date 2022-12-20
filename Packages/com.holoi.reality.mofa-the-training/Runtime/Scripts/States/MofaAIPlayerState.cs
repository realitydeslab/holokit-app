namespace Holoi.Reality.MOFATheTraining
{
    public abstract class MofaAIPlayerState
    {
        public abstract void OnEnter(MofaAIPlayer player);

        public abstract void OnUpdate(MofaAIPlayer player);

        public abstract void OnExit(MofaAIPlayer player);
    }
}
