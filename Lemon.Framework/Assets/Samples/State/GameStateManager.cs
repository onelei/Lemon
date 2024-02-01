using System.Collections.Generic;

namespace Lemon.Framework
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
    public class GameStateManager
    {
        private static Dictionary<int, GameState> GameStageGroup = new Dictionary<int, GameState>((int)EGameStage.Num);
        private static int eCurGameStage;
        private static int ePreGameStage;


        public static void Init()
        {
            GameStageGroup.Add((int)EGameStage.Entry, new EntryState());
            GameStageGroup.Add((int)EGameStage.Login, new LoginState());
            GameStageGroup.Add((int)EGameStage.HotUpdate, new DownloadState());
            GameStageGroup.Add((int)EGameStage.City, new LobbyState());
            GameStageGroup.Add((int)EGameStage.Battle, new BattleState());
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
