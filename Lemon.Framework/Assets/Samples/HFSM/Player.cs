using Lemon.Framework.HFSM;

namespace Lemon.Framework.Samples.HFSM
{
    public class Player
    {
        public HFsm<Player> HFsm;

        public Player()
        {
            HFsm = new HFsm<Player>(this);
            HFsm.AddState(new IdleState());
            HFsm.AddState(new WalkingState());
        }

        public void Update()
        {
            HFsm.Update();
        }
    }
}