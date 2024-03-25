namespace Lemon.Framework.UI.Widgets.SortOrderGroup
{
    public class LemonSortOrderItem : BaseBehavior
    {
        public int SortOrder = 0;
        
        public void SetSortOrder(int sortOrder)
        {
            SortOrder = sortOrder;
        }
        
        public int GetSortOrder()
        {
            return SortOrder;
        }
        
        public void SetSiblingIndex(int index)
        {
            CacheTransform.SetSiblingIndex(index);
        }
        
        
    }
}