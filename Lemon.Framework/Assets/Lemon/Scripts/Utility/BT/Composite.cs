/*
 * Composite Node，其实它按复合性质还可以细分为3种：
 * =>Selector Node:一真则真,全假则假;
 * =>Sequence Node:一假则假,全真则真;
 * =>Parallel Node:并发执行;
 */

using System.Collections.Generic;

namespace Lemon.Framework.BT
{
    public class Composite : Node
    {
        protected List<Node> children;
        public Composite()
        {
            children = new List<Node>();
        }

        public void addChild(Node node)
        {
            this.children.Add(node);
        }

        public void removeChild(Node node)
        {
            this.children.Remove(node);
        }

        public void removeAllChild()
        {
            this.children.Clear();
        }
    }
}


