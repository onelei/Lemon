---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by onelei.
--- DateTime: 2020/8/22 20:11
---

TimeMgr = {}

local unity_Time = CS.UnityEngine.Time
local realtimeSinceStartup

local __Time_Pool
local __Target

function TimeMgr.Init()
    __Time_Pool = {}
    __Target = {}
end

function TimeMgr.Add(time_delay,func_delay,target)
    target = target or __Target
    local targetPool = __Time_Pool[target]
    if not targetPool then
        targetPool = {}
    end
    local delayItem = {time = 0,time_delay = time_delay,func_delay = func_delay,target = target}
   table.insert(targetPool,delayItem)
    __Time_Pool[target] = targetPool
end

function TimeMgr.Remove(time_delay,func_delay,target)
    local delayPool = __Time_Pool[target]
    if delayPool then
        for i = 1, #delayPool do
            local delayItem = delayPool[i]
            if delayItem.time_delay == time_delay and delayItem.func_delay == func_delay then
                table.remove(delayPool,i)
                break
            end
        end
    end
end

function TimeMgr.RemoveByTarget(target)
    target = target or __Target
    local targetPool = __Time_Pool[target]
    if targetPool then
        __Time_Pool[target] = nil
    end
end

function TimeMgr.Update()
    local deltaTime = unity_Time.time
    for k, v in pairs(__Time_Pool) do
        local delayPool = v
        for i = #delayPool, 1, -1 do
            local delayItem = delayPool[i]
            delayItem.time = delayItem.time + deltaTime
            if delayItem.time > delayItem.time_delay then
                if delayItem.func_delay then
                    delayItem.func_delay()
                end
                table.remove(delayPool,i,delayItem)
            end
        end
    end
end

function TimeMgr.RemoveDelayItem(delayItem)
    --if not delayItem then
    --    return
    --end
    --local target = delayItem.target
    --local delayPool = __Time_Pool[target]
    --if delayPool then
    --    for i = #delayPool, 1, -1 do
    --        table.remove(delayPool,delayItem)
    --    end
    --end
end

function TimeMgr.GetRealtimeSinceStartup()
    return unity_Time.realtimeSinceStartup
end

TimeMgr.Init()

return TimeMgr