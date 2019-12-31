using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace Lemon.UI
{
    public struct EUICompare : IEqualityComparer<EUI>
    {
        public bool Equals(EUI x, EUI y)
        {
            return (int)x == (int)y;
        }

        public int GetHashCode(EUI obj)
        {
            return (int)obj;
        }
    }

    public sealed partial class UIManager : Singleton<UIManager>
    {
        private Dictionary<EUI, UIBase> UIBaseDict = new Dictionary<EUI, UIBase>(new EUICompare());

        private Stack<UIBase> uIBasesStack = new Stack<UIBase>();
        private Stack<EUI> eUIStack = new Stack<EUI>();

        public void Initial()
        {

        }

        public void Open(EUI eUI, object objs)
        {
            if (!UIBaseDict.ContainsKey(eUI))
            {
                UIBase uIBase = UnityEditor.AssetDatabase.LoadAssetAtPath<UIBase>("");
                if (uIBase == null)
                {
                   // QLog.LogError(StringPool.Concat("not find UIBase Script in EUI = ", eUI.ToString()));
                    return;
                }
                uIBasesStack.Pop();
                return;
            }
        }

        public void Close(EUI eUI)
        {
            //取出栈顶的界面，执行exit函数
            if (uIBasesStack.Count > 0)
            {
                UIBase uIBase = uIBasesStack.Pop();
                uIBase.OnExit();
            }

            if (eUIStack.Count > 0)
            {
                eUIStack.Pop();
            }

            //栈顶的界面，执行resume函数
            if (uIBasesStack.Count > 0)
            {
                UIBase uIBase = uIBasesStack.Peek();
                uIBase.OnResume();
            }
        }

        public bool IsOpen(EUI eUI)
        {
            return eUIStack.Contains(eUI);
        }
    }
}
