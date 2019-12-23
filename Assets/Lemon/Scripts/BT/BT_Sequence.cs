/*
 * Sequence Node
 * 当执行本类型Node时，它将从begin到end迭代执行自己的Child Node:
 * 如遇到一个Child Node执行后返回False，那停止迭代，
 * 本Node向自己的Parent Node也返回False；否则所有Child Node都返回True，
 * 那本Node向自己的Parent Node返回True。
 */

namespace Lemon.BT
{
    public class BT_Sequence : BT_Composite
    {
        private int index;

        public BT_Sequence()
        {
            reset();
        }

        public override BT_Result doAction()
        {
            if (this.children == null || this.children.Count == 0)
            {
                return BT_Result.SUCCESSFUL;
            }

            if (this.index >= this.children.Count)
            {
                reset();
            }

            BT_Result _result = BT_Result.NONE;
            for (int length = this.children.Count; index < length; ++index)
            {
                _result = this.children[index].doAction();
                if (_result == BT_Result.FAIL)
                {
                    reset();
                    return _result;
                }
                else if (_result == BT_Result.RUNING)
                {
                    return _result;
                }
                else
                {
                    continue;
                }

            }

            reset();
            return BT_Result.SUCCESSFUL;
        }

        private void reset()
        {
            this.index = 0;
        }
    }


}
