# Lua的String字符串拼接

每个语言都会遇到字符串拼接的问题。上回说到C#的字符串拼接Concat，我们知道C#中拼接许多字符串一般不用“+”号，因为每次+操作都会产生一个临时的字符串。所以C#里面就提供了StringBuilder--可变字符串来拼接，直到最后tostring的时候才会产生最终的string字符串。

Lua语言里面默认是”  ..“两个英文点号来表示字符串的拼接。但是当我们需要拼接多个字符串的时候，同样的思路table.concat

```
table.concat (list [, sep [, i [, j]]])
```

Lua提供一个列表，其所有元素都是字符串或数字，返回字符串 `list[i]..sep..list[i+1] ··· sep..list[j]`。 `sep` 的默认值是空串， `i` 的默认值是 1 ， `j` 的默认值是 `#list` 。 如果 `i` 比 `j` 大，返回空串。

也就是说我们可以将下面这段代码

```
function Concat(...)
    local origin = {...}
    local message = ""
    for i,v in pairs(origin) do
       message = message .. v
    end
    return message
end
```

改成

```
function Concat(...)
    local message = {...}
    return table.concat(message)
end
```

## 原理

这里参考https://www.lua.org/pil/11.6.html网站上面的示例，简单介绍一下table.concat 

假设现在正在逐行读取文件

```
  -- WARNING: bad code ahead!!
    local buff = ""
    for line in io.lines() do
    buff = buff .. line .. "\n"
    end
```

Lua使用了真正的垃圾收集算法；当它检测到程序正在使用过多的内存时，它将遍历其所有数据结构并释放那些不再使用的结构（垃圾）。

让我们假设我们处于读取循环的中间。 `buff`已经是一个50 KB的字符串，每行有20个字节。当Lua串联时`buff..line.."\n"`，它将创建一个具有50,020字节的新字符串，并将50 KB复制`buff`到该新字符串中。也就是说，对于每条新行，Lua都会移动50 KB的内存并不断增长。读取100行后（仅2 KB），Lua已经移动了5 MB以上的内存。

```
    buff = buff .. line .. "\n"
```

经过两个循环之后，有两个旧字符串构成了总计超过100 KB的垃圾。因此，Lua相当正确地决定，现在是运行其垃圾收集器的好时机，因此它释放了这100 KB。问题是，这将每两个周期发生一次，因此Lua将在读取整个文件之前运行其垃圾收集器2000次。即使完成所有这些工作，它的内存使用量也将大约是文件大小的三倍。

**使用concat，我们可以简单地将所有字符串收集在一个表中，然后一次将它们全部连接起来。 因为concat使用C实现，所以即使对于大字符串也很有效。**

然后将上面的代码改造一下

```
local t = {}
for line in io.lines() do
  	table.insert(t, line)
end
s = table.concat(t, "\n") .. "\n"
```

 综上：Lua连大量字符串的时候使用table.concat



参考：https://www.lua.org/pil/11.6.html

参考：http://cloudwu.github.io/lua53doc/manual.html#pdf-table.concat