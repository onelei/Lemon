namespace LemonFramework
{
    public interface IState
    {
        void OnEnter();

        void OnExit();
    }
}