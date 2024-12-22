/*
 * Composite Node，其实它按复合性质还可以细分为3种：
 * =>Selector Node:一真则真,全假则假;
 * =>Sequence Node:一假则假,全真则真;
 * =>Parallel Node:并发执行;
 */

using System.Collections.Generic;

namespace LemonFramework.Runtime.Behavior
{
    public class Composite : Node
    {
        protected readonly List<Node> Children;

        protected Composite()
        {
            Children = new List<Node>();
        }

        public void AddChild(Node node)
        {
            this.Children.Add(node);
        }

        public void RemoveChild(Node node)
        {
            this.Children.Remove(node);
        }

        public void RemoveAllChild()
        {
            this.Children.Clear();
        }
    }
}


