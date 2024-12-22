/*
 * Parallel Node
 * 并发执行它的所有Child Node.
 * 而向Parent Node返回的值和Parallel Node所采取的具体策略相关;
 * Parallel Selector Node:一False则返回False,全True才返回True.
 */

using System.Collections.Generic;

namespace LemonFramework.Runtime.Behavior
{
    public class ParallelSelector : Parallel
    {
        private readonly List<Node> runningNodes;
        private bool isFail;
        
        public ParallelSelector()
        {
            runningNodes = new List<Node>();
            isFail = false;
        }

        public override Result OnAction()
        {
            if (Children == null || Children.Count == 0)
            {
                return Result.Successful;
            }

            Result result;
            var list = ListPool<Node>.Get();
            var nodes = runningNodes.Count > 0 ? runningNodes : Children;
            for (int i = 0, length = nodes.Count; i < length; ++i)
            {
                result = nodes[i].OnAction();
                switch (result)
                {
                    case Result.Successful:
                        break;
                    case Result.Runing:
                        list.Add(nodes[i]);
                        break;
                    default:
                        isFail = true;
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
            return isFail ? Result.Fail : Result.Successful;
        }

        private void Reset()
        {
            runningNodes.Clear();
            isFail = false;
        }
    }
}


