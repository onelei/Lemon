/*
 * Parallel Node
 * 并发执行它的所有Child Node.
 * 而向Parent Node返回的值和Parallel Node所采取的具体策略相关;
 * Parallel Sequence Node:一True则返回True,全False才返回False.
 */

using System.Collections.Generic;

namespace LemonFramework.Runtime.Behavior
{
    public class ParallelSequence : Parallel
    {
        private readonly List<Node> runningNodes;
        private bool isSuccess;
        
        public ParallelSequence()
        {
            runningNodes = new List<Node>();
            isSuccess = false;
        }

        public override Result OnAction()
        {
            if (Children == null || Children.Count == 0)
            {
                return Result.Successful;
            }

            var list = ListPool<Node>.Get();
            var nodes = runningNodes.Count > 0 ? runningNodes : Children;
            Result result;
            for (int i = 0, length = nodes.Count; i < length; ++i)
            {
                result = nodes[i].OnAction();
                switch (result)
                {
                    case Result.Successful:
                        isSuccess = true;
                        break;
                    case Result.Runing:
                        list.Add(nodes[i]);
                        break;
                }
            }

            // 存在等待节点就返回等待;
            if (list.Count > 0)
            {
                runningNodes.Clear();
                runningNodes.AddRange(list);           
                ListPool<Node>.Release(list);
                return Result.Runing;
            }

            // 检查返回结果;
            result = CheckResult();
            Reset();
            return result;
        }

        private Result CheckResult()
        {
            return isSuccess ? Result.Successful : Result.Fail;
        }

        private void Reset()
        {
            runningNodes.Clear();
            isSuccess = false;
        }
    }

}
