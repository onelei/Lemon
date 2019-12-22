/*
 * Parallel Node
 * 并发执行它的所有Child Node.
 * 而向Parent Node返回的值和Parallel Node所采取的具体策略相关;
 * Parallel Selector Node:一False则返回False,全True才返回True.
 */

using System.Collections.Generic;

namespace Lemon.BT
{
    public class Bt_ParallelSelector : BT_Parallel
    {
        private List<BT_Node> m_pWaitNodes;
        private bool m_pIsFail;
        public Bt_ParallelSelector()
        {
            m_pWaitNodes = new List<BT_Node>();
            m_pIsFail = false;
        }

        public override BT_Result doAction()
        {
            if (this.children == null || this.children.Count == 0)
            {
                return BT_Result.SUCCESSFUL;
            }

            BT_Result _result = BT_Result.NONE;
            List<BT_Node> _waitNodes = ListPool<BT_Node>.Get();
            List<BT_Node> _mainNodes = ListPool<BT_Node>.Get();
            _mainNodes = this.m_pWaitNodes.Count > 0 ? this.m_pWaitNodes : this.children;
            for (int i = 0, length = _mainNodes.Count; i < length; ++i)
            {
                _result = _mainNodes[i].doAction();
                switch (_result)
                {
                    case BT_Result.SUCCESSFUL:
                        break;
                    case BT_Result.RUNING:
                        _waitNodes.Add(_mainNodes[i]);
                        break;
                    default:
                        m_pIsFail = true;
                        break;
                }
            }
            ListPool<BT_Node>.Release(_mainNodes);


            // 存在等待节点就返回等待;
            if (_waitNodes.Count > 0)
            {
                this.m_pWaitNodes.Clear();
                this.m_pWaitNodes.AddRange(_waitNodes);
                ListPool<BT_Node>.Release(_waitNodes);
                return BT_Result.RUNING;
            }

            // 检查返回结果;
            _result = checkResult();
            reset();
            return _result;
        }

        private BT_Result checkResult()
        {
            return m_pIsFail ? BT_Result.FAIL : BT_Result.SUCCESSFUL;
        }

        private void reset()
        {
            this.m_pWaitNodes.Clear();
            this.m_pIsFail = false;
        }
    }
}


