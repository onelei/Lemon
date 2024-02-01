/*
 * Sequence Node
 * 当执行本类型Node时，它将从begin到end迭代执行自己的Child Node:
 * 如遇到一个Child Node执行后返回False，那停止迭代，
 * 本Node向自己的Parent Node也返回False；否则所有Child Node都返回True，
 * 那本Node向自己的Parent Node返回True。
 */

namespace Lemon.Framework.BT
{
    public class Sequence : Composite
    {
        private int index;

        public Sequence()
        {
            reset();
        }

        public override Result doAction()
        {
            if (this.children == null || this.children.Count == 0)
            {
                return Result.Successful;
            }

            if (this.index >= this.children.Count)
            {
                reset();
            }

            Result _result = Result.None;
            for (int length = this.children.Count; index < length; ++index)
            {
                _result = this.children[index].doAction();
                if (_result == Result.Fail)
                {
                    reset();
                    return _result;
                }
                else if (_result == Result.Runing)
                {
                    return _result;
                }
                else
                {
                    continue;
                }

            }

            reset();
            return Result.Successful;
        }

        private void reset()
        {
            this.index = 0;
        }
    }


}
