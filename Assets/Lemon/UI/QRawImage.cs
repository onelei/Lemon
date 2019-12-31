using UnityEngine;
using UnityEngine.UI;

namespace Lemon.UI
{
    [AddComponentMenu("UI/QRawImage")]
    public class QRawImage : RawImage
    {
        [HideInInspector]
        public bool bInit = false;
        /// <summary>
        /// 多语言key
        /// </summary>
        public string key = string.Empty;
    }
}
