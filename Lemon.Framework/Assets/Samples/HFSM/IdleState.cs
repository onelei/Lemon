using LemonFramework.HFSM;
using LemonFramework.Log;
 
namespace LemonFramework.Samples.HFSM
{
    public class IdleState : IState<Player>
    {
        private readonly ILogger logger = LogManager.GetLogger(nameof(IdleState));
        
        public void Enter(Player owner)
        {
            logger.Log("Entering Idle State");
        }

        public void Execute(Player owner)
        {
            logger.Log("Idle State Executing");
        }

        public void Exit(Player owner)
        {
            logger.Log("Exiting Idle State");
        }
    }

}