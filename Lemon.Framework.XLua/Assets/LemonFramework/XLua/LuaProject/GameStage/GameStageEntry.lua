---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by onelei.
--- DateTime: 2020/9/6 20:13
---

---@class GameStageEntry

local GameStageBase = require("GameStage.GameStageBase")
local GameStageEntry = class("GameStageEntry",GameStageBase)

function GameStageEntry:OnEnter()
    GameStageBase.OnEnter(self)

    require("Common.TimeMgr")
    require("Common.LuaBehavior")
    require("Common.StepAction.StepUtility")

    require("UIMgr.UIType")
    require("UIMgr.UIMgr")

    UIMgr.Init()

    GameStageUtility.Enter(GameStageUtility.EGameStage.Login)

end

function GameStageEntry:OnExit()
    GameStageBase.OnExit(self)
end

return GameStageEntry