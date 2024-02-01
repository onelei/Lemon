/*
 * Action Node
 * 它完成具体的一次(或一个step)的行为，视需求返回值;
 * 叶子节点;
 */

namespace Lemon.Framework.BT
{
    public class Action : Node
    {
        public override Result doAction()
        {
            return Result.Fail;
        }
    }
}
