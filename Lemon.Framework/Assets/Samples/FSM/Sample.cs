using Lemon.Framework.FSM;

namespace Lemon.Framework.Samples.FSM
{
    public class Sample
    {
        public enum State
        {
            Idle,
            Running,
            Paused
        }

        public enum Trigger
        {
            Start,
            Pause,
            Resume,
            Stop
        }

        public static void Main(string[] args)
        {
            var stateMachine = new Fsm<State, Trigger>(State.Idle);

            // 添加状态转换
            stateMachine.AddTransition(State.Idle, Trigger.Start, State.Running);
            stateMachine.AddTransition(State.Running, Trigger.Pause, State.Paused);
            stateMachine.AddTransition(State.Paused, Trigger.Resume, State.Running);
            stateMachine.AddTransition(State.Running, Trigger.Stop, State.Idle);
            stateMachine.AddTransition(State.Paused, Trigger.Stop, State.Idle);

            // 触发事件，改变状态
            stateMachine.Fire(Trigger.Start); // 输出: Transitioned to state: Running
            stateMachine.Fire(Trigger.Pause); // 输出: Transitioned to state: Paused
            stateMachine.Fire(Trigger.Resume); // 输出: Transitioned to state: Running
            stateMachine.Fire(Trigger.Stop); // 输出: Transitioned to state: Idle
        }
    }
}