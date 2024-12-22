namespace LemonFramework.Samples.HFSM
{
    public class Sample
    {
        static void Main(string[] args)
        {
            Player player = new Player();

            player.HFsm.ChangeState<IdleState>();
            player.Update();

            player.HFsm.ChangeState<WalkingState>();
            player.Update();
        }
    }
}