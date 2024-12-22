/*
 * Parallel Node
 * 并发执行它的所有Child Node.
 * 而向Parent Node返回的值和Parallel Node所采取的具体策略相关;
 * Parallel Selector Node:一False则返回False,全True才返回True.
 * Parallel Sequence Node:一True则返回True,全False才返回False.
 * Parallel Node:提供了并发,提高性能;
 * 不需要像Selector/Sequence那样预判哪个Child Node应摆前,哪个应摆后;
 * 常见情况是;
 * (1)用于并行多棵Action子树.
 * (2)在Parallel Node下挂一棵子树,并挂上多个Condition Node;
 * 以提供实时性和性能;
 * Parallel Node增加性能和方便性的同时,也增加实现和维护复杂度;
 */

namespace LemonFramework.Runtime.Behavior
{
    public class Parallel : Composite
    {
        public Parallel()
        {

        }
    }

}

