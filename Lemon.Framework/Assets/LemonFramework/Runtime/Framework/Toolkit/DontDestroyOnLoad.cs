using Lemon.Framework.UI.Widgets;

namespace Lemon.Framework
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
