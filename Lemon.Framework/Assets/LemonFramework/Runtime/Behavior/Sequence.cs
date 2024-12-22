/*
 * Sequence Node
 * 当执行本类型Node时，它将从begin到end迭代执行自己的Child Node:
 * 如遇到一个Child Node执行后返回False，那停止迭代，
 * 本Node向自己的Parent Node也返回False；否则所有Child Node都返回True，
 * 那本Node向自己的Parent Node返回True。
 */

namespace LemonFramework.Runtime.Behavior
{
    public class Sequence : Composite
    {
        private int index;

        public Sequence()
        {
            Reset();
        }

        public override Result OnAction()
        {
            if (Children == null || Children.Count == 0)
            {
                return Result.Successful;
            }

            if (index >= Children.Count)
            {
                Reset();
            }

            for (int length = Children.Count; index < length; ++index)
            {
                var result = Children[index].OnAction();
                switch (result)
                {
                    case Result.Fail:
                        Reset();
                        return result;
                    case Result.Runing:
                        return result;
                }
            }

            Reset();
            return Result.Successful;
        }

        private void Reset()
        {
            index = 0;
        }
    }


}
