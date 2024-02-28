﻿using UnityEngine;
using Lemon.Framework.BT;
using Lemon.Framework.Extension;

namespace Lemon.Framework.SkillSystem
{
    public class SkillUnitMoveNode : SkillActionNode
    {
        private SkillUnit skillUnit;
        private Vector3 targetPosition;
        private Vector3 selfOriginPosition;

        private Vector3 perDistance;
        private bool bStart = false;

        public SkillUnitMoveNode(SkillUnit skillUnit, Vector3 targetPosition)
        {
            this.skillUnit = skillUnit;
            this.targetPosition = targetPosition;
        }

        public override Result doAction()
        {
            //该action会循环利用，因此每次结束之后，重置一下。
            if(!bStart)
            {
                Reset();
                bStart = true;
            }

            if (skillUnit == null)
            {
                Reset();
                return Result.Fail;
            }
            Vector3 selfPosition = skillUnit.transform.localPosition;

            if (selfPosition.Approximately(targetPosition))
            {
                Reset();
                return Result.Successful;
            }

            //如果剩余距离小于每次的最小距离，则直接设置目的地
            float leftDistance = Vector3.Distance(targetPosition, selfPosition);
            if (leftDistance <= skillUnit.Speed)
            {
                skillUnit.transform.localPosition = targetPosition;
                Reset();
                return Result.Successful;
            }

            //设置行走的位置
            skillUnit.transform.localPosition = selfPosition + perDistance;
            return Result.Runing;
        }

        public void Reset()
        {
            Vector3 myPosition = skillUnit.transform.localPosition;
            Vector3 distanceDir = (targetPosition - myPosition).normalized;
            perDistance = skillUnit.Speed * distanceDir;
            bStart = false;
        }
    }
}
