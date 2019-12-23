using Lemon.BT;
using System.Collections.Generic;
using UnityEngine;

namespace Lemon.SkillSystem
{
    public partial class SkillSystemManager : Singleton<SkillSystemManager>
    {
        public Dictionary<int, SkillUnit> dictSkillUnits = new Dictionary<int, SkillUnit>();

        public void RegisterSkillUnit(int ID, SkillUnit skillUnit)
        {
            if (dictSkillUnits.ContainsKey(ID))
                return;
            dictSkillUnits.Add(ID, skillUnit);
        }

        public void onStart()
        {

        }

        public void onUpdate()
        {
            Dictionary<int, SkillUnit>.Enumerator enumerator = dictSkillUnits.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Value.Running();
            }
        }

        public void Run(ESkillType skillSystemTriggerType, params object[] objs)
        {
            Dictionary<int, SkillUnit>.Enumerator enumerator = dictSkillUnits.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Value.Run(skillSystemTriggerType, objs);
            }
        }
    }
}
