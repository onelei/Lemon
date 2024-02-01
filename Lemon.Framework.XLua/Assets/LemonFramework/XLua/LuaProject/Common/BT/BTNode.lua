---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by onelei.
--- DateTime: 2020/8/8 22:55
---
---* Parent Node
---* 任何Node被执行后，必须向其Parent Node报告执行结果：成功 / 失败
---* 这简单的成功 / 失败汇报原则被很巧妙地用于控制整棵树的决策方向
---* =>Composite Node
---* =>Decorator Node
---* =>Condition Node
---* =>Action Node


---@class BTNode
local BTNode = class("BTNode")

function BTNode:ctor(Opts)
    if Opts then
        self.Opts = Opts
        self.Opts.Condition = Opts.Condition
        self.Opts.BlackBoard = Opts.BlackBoard
    end
end

function BTNode:DoAction()
    return LuaDef.BTResult.NONE
end

return BTNode