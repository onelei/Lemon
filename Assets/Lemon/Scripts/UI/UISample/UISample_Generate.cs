using Lemon.UI;
using UnityEngine;

namespace Lemon.UI
{
    public sealed partial class UISample : UIBase
    {
        //==自动化变量开始
        public QButton QButton_Close; 
 



#if UNITY_EDITOR
        [ContextMenu("GeneratePathEditor")]
        public override void GeneratePathEditor()
        {
            //==自动化路径开始
            QButton_Close = CacheTransform.Find("GameObject/GameObject (1)/QButton_Close/").GetComponent<QButton>();
 
           
        }        
#endif
    }
}
