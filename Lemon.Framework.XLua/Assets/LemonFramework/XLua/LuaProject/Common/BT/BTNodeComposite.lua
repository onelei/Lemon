---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by onelei.
--- DateTime: 2020/8/8 23:07
---
---* Composite Node，其实它按复合性质还可以细分为3种：
---* =>Selector Node:一真则真,全假则假
---* =>Sequence Node:一假则假,全真则真
---* =>Parallel Node:并发执行

---@class BTNodeComposite
local BTNode = require("Common.BT.BTNode")
local BTNodeComposite = class("BTNodeComposite",BTNode)

function BTNodeComposite:ctor(Opts)
    BTNode.ctor(self,Opts)
    self.Children = {}
end

function BTNodeComposite:Add(node)
    table.insert(self.Children,node)
end

function BTNodeComposite:Remove(node)
    for i=#self.Children,1,-1 do
        if self.Children[i] == node then
            table.remove(self.Children, i)
            return
        end
    end
end

function BTNodeComposite:Clear()
    self.Children = {}
end

return BTNodeComposite