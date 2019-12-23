/*
 * 技能单位
 */
using Lemon.BT;
using Lemon.SkillSystem;
using UnityEngine;

namespace Lemon
{
    public partial class SkillUnit : Unit
    {
        bool bRun = false;
        /// <summary>
        /// 组合节点
        /// </summary>
        BT_Sequence SequenceNode = new BT_Sequence();
        BT_Result Result = BT_Result.NONE;

        public readonly Vector3 ORIGIN_POSIITON = new Vector3(-10f,0f,0f);

        public Vector3 SetPosition(Vector3 position)
        {
            return transform.localPosition = position;
        }

        /// <summary>
        /// 激活行为树
        /// </summary>
        protected override void MonoStart()
        {
            SetPosition(ORIGIN_POSIITON);
            SkillSystemManager.Instance.RegisterSkillUnit(ID, this);
            Result = BT_Result.NONE;
            bRun = false;
        }

        public void Running()
        {
            if (!bRun)
                return;
            Result = SequenceNode.doAction();
            //执行结束
            if(Result == BT_Result.FAIL || Result == BT_Result.SUCCESSFUL)
            {
                bRun = false;
            }
        }

        public bool Run(ESkillType eSkillType, params object[] objs)
        {
            if(bRun)
            {
                //如果当前是Result 是Running状态，判断是否可以打断
                return false;
            }
            switch (eSkillType)
            {
                case ESkillType.CHARGE:
                    Charge();//通用的节点后面可以循环利用
                    break;
                case ESkillType.MOVE:
                    Move();
                    break;
                default:
                    break;
            }
            bRun = true;
            return true;
        }

        /// <summary>
        /// 移动
        /// </summary>
        void Move()
        {
            SetPosition(ORIGIN_POSIITON);

            SequenceNode.removeAllChild();
            //移动
            BT_Node moveNode = new SkillUnitMoveNode(this, new Vector3(0, 0, 0));
            SequenceNode.addChild(moveNode); 
        }

        /// <summary>
        /// 冲锋
        /// </summary>
        void Charge()
        {
            SetPosition(ORIGIN_POSIITON);

            SequenceNode.removeAllChild();
            //移动1->冲出去
            BT_Node moveNode = new SkillUnitMoveNode(this, new Vector3(0, 0, 0));
            SequenceNode.addChild(moveNode);

            //移动2->回来
            BT_Node moveNode2 = new SkillUnitMoveNode(this, ORIGIN_POSIITON);
            SequenceNode.addChild(moveNode2);
        }
    }
}
