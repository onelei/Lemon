using UnityEngine.UI;
using UnityEngine;

namespace Lemon.UI
{
    public class UISkillMain: BaseMonoUIClass
    {
        public Button Button_Go;
        public Dropdown dropdown;

        protected override void MonoStart()
        {
            Button_Go.onClick.AddListener(ClickEventGo);
        }

        private void ClickEventGo()
        {
            switch (dropdown.value)
            {
                case 0:
                    {
                        //冲锋
                        SkillSystem.SkillSystemManager.Instance.Run(SkillSystem.ESkillType.CHARGE);
                        break;
                    }
                case 1:
                    {
                        //移动
                        SkillSystem.SkillSystemManager.Instance.Run(SkillSystem.ESkillType.MOVE);
                        break;
                    }
            }
        }
    }
}
