using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Lemon
{
    public static partial class Utility
    {
        public static bool Approximately(float delta, float accuracy = 0.001f)
        {
            if ((-accuracy) <= (delta) && (delta <= accuracy))
            {
                return true;
            }
            return false;
        }

        #region UnityEngine

        public static bool Approximately(this Vector3 origin, Vector3 target, float accuracy = 0.001f)
        {
            Vector3 delta = origin - target;
            if (Approximately(delta.x, accuracy) && Approximately(delta.y, accuracy) && Approximately(delta.z, accuracy))
            {
                return true;
            }
            return false;
        }

        public static T GetOrAddCompoment<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }

        public static void AddListenerOnce(this Button.ButtonClickedEvent unityEvent, UnityEngine.Events.UnityAction unityAction)
        {
            unityEvent.RemoveListener(unityAction);
            unityEvent.AddListener(unityAction);
        }

        public static void AddListenerOnly(this Button.ButtonClickedEvent unityEvent, UnityEngine.Events.UnityAction unityAction)
        {
            unityEvent.RemoveAllListeners();
            unityEvent.AddListener(unityAction);
        }

        public static void AddListenerOnce(this UnityEngine.Events.UnityEvent unityEvent, UnityEngine.Events.UnityAction unityAction)
        {
            unityEvent.RemoveListener(unityAction);
            unityEvent.AddListener(unityAction);
        }

        public static void AddListenerOnly(this UnityEngine.Events.UnityEvent unityEvent, UnityEngine.Events.UnityAction unityAction)
        {
            unityEvent.RemoveAllListeners();
            unityEvent.AddListener(unityAction);
        }
        #endregion
    }
}
