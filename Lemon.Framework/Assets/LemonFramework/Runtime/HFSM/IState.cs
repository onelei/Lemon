namespace LemonFramework.HFSM
{
    /// <summary>
    /// Define the state of the state machine
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IState<T>
    {
        void Enter(T owner);
        void Execute(T owner);
        void Exit(T owner);
    }
}