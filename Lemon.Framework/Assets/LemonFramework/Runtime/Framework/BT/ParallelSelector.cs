﻿/*
 * Parallel Node
 * 并发执行它的所有Child Node.
 * 而向Parent Node返回的值和Parallel Node所采取的具体策略相关;
 * Parallel Selector Node:一False则返回False,全True才返回True.
 */

using System.Collections.Generic;

namespace Lemon.Framework.BT
{
    public class ParallelSelector : Parallel
    {
        private List<Node> m_pWaitNodes;
        private bool m_pIsFail;
        public ParallelSelector()
        {
            m_pWaitNodes = new List<Node>();
            m_pIsFail = false;
        }

        public override Result doAction()
        {
            if (this.children == null || this.children.Count == 0)
            {
                return Result.Successful;
            }

            Result _result = Result.None;
            List<Node> _waitNodes = ListPool<Node>.Get();
            List<Node> _mainNodes = ListPool<Node>.Get();
            _mainNodes = this.m_pWaitNodes.Count > 0 ? this.m_pWaitNodes : this.children;
            for (int i = 0, length = _mainNodes.Count; i < length; ++i)
            {
                _result = _mainNodes[i].doAction();
                switch (_result)
                {
                    case Result.Successful:
                        break;
                    case Result.Runing:
                        _waitNodes.Add(_mainNodes[i]);
                        break;
                    default:
                        m_pIsFail = true;
                        break;
                }
            }
            ListPool<Node>.Release(_mainNodes);


            // 存在等待节点就返回等待;
            if (_waitNodes.Count > 0)
            {
                this.m_pWaitNodes.Clear();
                this.m_pWaitNodes.AddRange(_waitNodes);
                ListPool<Node>.Release(_waitNodes);
                return Result.Runing;
            }

            // 检查返回结果;
            _result = checkResult();
            reset();
            return _result;
        }

        private Result checkResult()
        {
            return m_pIsFail ? Result.Fail : Result.Successful;
        }

        private void reset()
        {
            this.m_pWaitNodes.Clear();
            this.m_pIsFail = false;
        }
    }
}


