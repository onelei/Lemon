using LemonFramework.UI.Widgets;

namespace LemonFramework
{
    public class DontDestroyOnLoad : BaseBehavior
    {
        private void Awake()
        {
            CacheGameObject.isStatic = true;
            DontDestroyOnLoad(CacheGameObject);
        }
    }
}
