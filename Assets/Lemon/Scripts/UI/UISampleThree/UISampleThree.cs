/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using Lemon.UI;
using UnityEngine;

namespace Lemon.UI
{
    public sealed partial class UISampleThree : UIBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            QButton_Close.onClick.AddListener(() =>
            {
                UIManager.Instance.Close(EUI.UISampleThree);
            });
        }
    }
}
