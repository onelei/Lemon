using UnityEngine;
using UnityEngine.UI;

namespace Lemon.UI
{
    [AddComponentMenu("UI/QImage")]
    public class QImage : Image
    {
        [HideInInspector]
        public bool bInit = false;
        /// <summary>
        /// 多语言key
        /// </summary>
        public string key = string.Empty;
    }
}
