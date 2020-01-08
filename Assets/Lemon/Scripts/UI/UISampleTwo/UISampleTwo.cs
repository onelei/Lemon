using Lemon.UI;
using UnityEngine;

namespace Lemon.UI
{
    public sealed partial class UISampleTwo : UIBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            QButton_Close.onClick.AddListener(() =>
            {
                UIManager.Instance.Open(EUI.UISampleThree);
            });
        }
    }
}
