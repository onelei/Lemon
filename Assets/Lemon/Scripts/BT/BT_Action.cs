/*
 * Action Node
 * 它完成具体的一次(或一个step)的行为，视需求返回值;
 * 叶子节点;
 */

namespace Lemon.BT
{
    public class BT_Action : BT_Node
    {
        public override BT_Result doAction()
        {
            return BT_Result.FAIL;
        }
    }
}
