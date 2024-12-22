/*
 * Condition node.
 */

namespace LemonFramework.Runtime.Behavior
{
    public class Condition : Node
    {
        public override Result OnAction()
        {
            return Result.Fail;
        }
    }
} 


