namespace Lemon.Framework
{
    public interface IState
    {
        void OnEnter();

        void OnExit();
    }
}