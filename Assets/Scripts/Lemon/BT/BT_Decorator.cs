/*
 * Decorator node.
 */

using System.Collections.Generic;

namespace Lemon.BT
{
    public class BT_Decorator : BT_Node
    {
        private BT_Node child;

        public BT_Decorator()
        {
            child = null;
        }

        protected void setChild(BT_Node node)
        {
            this.child = node;
        }
    }

}

