namespace Lemon.Framework
{
    public class Singleton<T> where T : class, new()
    {
        public static T Instance
        {
            get
            {
                //获取的时候再创建（剑指Offer）
                return Nested._instance;
            }
        }

        class Nested
        {
            internal static T _instance = new T();
        }
    }
}