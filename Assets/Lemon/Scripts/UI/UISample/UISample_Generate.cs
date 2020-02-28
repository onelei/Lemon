/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using Lemon.UI;
using UnityEngine;

namespace Lemon.UI
{
    public sealed partial class UISample : UIBase
    {
        //==自动化变量开始
        public QText QText_Title; 
        public QButton QButton_Close; 
 



#if UNITY_EDITOR
        [ContextMenu("GeneratePathEditor")]
        public override void GeneratePathEditor()
        {
            //==自动化路径开始
            QText_Title = CacheTransform.Find("Center/QText_Title/").GetComponent<QText>();
            QButton_Close = CacheTransform.Find("Center/ButtonGroup/QButton_Close/").GetComponent<QButton>();
 
           
        }        
#endif
    }
}
