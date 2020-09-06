using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lemon
{
    public enum EGameStage
    {
        None,
        Entry,
        Login,
        HotUpdate,
        City,
        Battle,
        Num,
    }

    /// <summary>
    /// 游戏主状态机
    /// </summary>
    public class GameStageMgr
    {
        private static Dictionary<int, GameStageBase> GameStageGroup = new Dictionary<int, GameStageBase>((int)EGameStage.Num);
        private static int eCurGameStage;
        private static int ePreGameStage;


        public static void Init()
        {
            GameStageGroup.Add((int)EGameStage.Entry, new GameStageEntry());
            GameStageGroup.Add((int)EGameStage.Login, new GameStageLogin());
            GameStageGroup.Add((int)EGameStage.HotUpdate, new GameStageHotUpdate());
            GameStageGroup.Add((int)EGameStage.City, new GameStageCity());
            GameStageGroup.Add((int)EGameStage.Battle, new GameStageBattle());

        }


        public static void Enter(EGameStage eGameStage)
        {
            Enter((int)eGameStage);
        }

        public static void Enter(int eGameStage)
        {
            if (ePreGameStage == eGameStage)
                return;
            
            if(!GameStageGroup.ContainsKey(eGameStage))
                return;
            
            ePreGameStage = eCurGameStage;
            eCurGameStage = eGameStage;
            
            GameStageGroup[ePreGameStage].OnExit();
            GameStageGroup[eCurGameStage].OnEnter();

        }
    }
}
