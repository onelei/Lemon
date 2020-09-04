---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by onelei.
--- DateTime: 2020/8/22 15:35
---

---@class StepActionTest1
local StepActionBase = class("Common.StepAction.StepActionBase")
local StepActionTest1 = class("StepActionTest1",StepActionBase)

function StepActionTest1:SetOpts(...)
    Debug.Log("Opts: ",...)
    self.Opts = { ... }
    self.time = 0
end

function StepActionTest1:DoAction()
    self.time = self.time + 1
    Debug.Log("StepActionTest1: ",self.time)
    if self.time < 5 then
        return LuaDef.BTResult.RUNNING
    end
    return LuaDef.BTResult.SUCCESSFUL
end

return StepActionTest1