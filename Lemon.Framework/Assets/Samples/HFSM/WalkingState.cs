using LemonFramework.HFSM;
using LemonFramework.Log;

namespace LemonFramework.Samples.HFSM
{
    public class WalkingState : IState<Player>
    {
        private readonly ILogger logger = LogManager.GetLogger(nameof(IdleState));
        
        public void Enter(Player owner)
        {
            logger.Log("Entering Walking State");
        }

        public void Execute(Player owner)
        {
            logger.Log("Walking State Executing");
        }

        public void Exit(Player owner)
        {
            logger.Log("Exiting Walking State");
        }
    }
}