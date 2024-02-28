using UnityEngine.Events;

namespace Lemon.Framework.Extension
{
    public static class UnityEventExtension
    {
        public static void AddOnceListener(this UnityEvent unityEvent, UnityAction unityAction)
        {
            unityEvent.RemoveListener(unityAction);
            unityEvent.AddListener(unityAction);
        }

        public static void AddOnlyListener(this UnityEvent unityEvent, UnityAction unityAction)
        {
            unityEvent.RemoveAllListeners();
            unityEvent.AddListener(unityAction);
        }
    }
}