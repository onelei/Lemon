
namespace Lemon
{
    public interface IGameStageBase
    {
        void OnEnter();

        void OnExit();
    }

    public class GameStageBase : IGameStageBase
    {
        public virtual void OnEnter()
        {

        }

        public virtual void OnExit()
        {

        }
    }
}