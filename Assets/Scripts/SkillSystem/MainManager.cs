using System;
using UnityEngine;
using Lemon.SkillSystem;

namespace Lemon
{
    public sealed class MainManager : MonoClass
    {
        public static float deltaTime;

        protected override void MonoStart()
        {
            SkillSystemManager.Instance.onStart();
        }

        protected override void MonoUpdate()
        {
            deltaTime += Time.deltaTime;
            SkillSystemManager.Instance.onUpdate();
        }
    }
}
