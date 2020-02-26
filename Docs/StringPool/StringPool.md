# 字符串优化加强版--StringPool

之前写过一篇string字符串优化相关的文章，但是里面是使用一个static静态变量。先看下之前的代码

```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lemon
{
    public class QString
    {
        private static StringBuilder CommonStringBuilder = new StringBuilder();
        private static StringBuilder InternalStringBuilder = new StringBuilder();

        public static StringBuilder GetStringBuilder()
        {
            CommonStringBuilder.Remove(0, CommonStringBuilder.Length);
            return CommonStringBuilder;
        }

        public static string Concat(string s1, string s2)
        {
            InternalStringBuilder.Remove(0, InternalStringBuilder.Length);
            InternalStringBuilder.Append(s1);
            InternalStringBuilder.Append(s2);
            return InternalStringBuilder.ToString();
        }

        public static string Concat(string s1, string s2, string s3)
        {
            InternalStringBuilder.Remove(0, InternalStringBuilder.Length);
            InternalStringBuilder.Append(s1);
            InternalStringBuilder.Append(s2);
            InternalStringBuilder.Append(s3);
            return InternalStringBuilder.ToString();
        }

        public static string Format(string src, params object[] args)
        {
            InternalStringBuilder.Remove(0, InternalStringBuilder.Length);
            InternalStringBuilder.AppendFormat(src, args);
            return InternalStringBuilder.ToString();
        }

    }
}
```

QString里面的stringbuilder一共是两个，一个是给外部用的是CommonStringBuilder，一个是内部使用的InternalStringBuilder。

```
private static StringBuilder CommonStringBuilder = new StringBuilder();
private static StringBuilder InternalStringBuilder = new StringBuilder();
```

之前一直是在Unity里面主线程上面使用的，没有用到多线程。昨天写东西用到了多线程，因此这里仅一个静态的stringbuilder已经无法满足需求了，因此StringPool产生了，底层实现其实是对stringbuilder使用了pool形式。

大家还记得之前的ObjectPool的文章么，今天就使用ObjectPool来封装一个StringBuilderPool。先贴上ObjectPool的代码。

```
/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using System.Collections.Generic;
using UnityEngine.Events;

public class ObjectPool<T> where T : new()
{
    private readonly Stack<T> m_Stack = new Stack<T>();
    private readonly UnityAction<T> m_ActionOnGet;
    private readonly UnityAction<T> m_ActionOnRelease;

    public int countAll { get; private set; }
    public int countActive { get { return countAll - countInactive; } }
    public int countInactive { get { return m_Stack.Count; } }

    public ObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
    {
        m_ActionOnGet = actionOnGet;
        m_ActionOnRelease = actionOnRelease;
    }

    public T Get()
    {
        T element;
        if (m_Stack.Count == 0)
        {
            element = new T();
            countAll++;
        }
        else
        {
            element = m_Stack.Pop();
        }
        if (m_ActionOnGet != null)
            m_ActionOnGet(element);
        return element;
    }

    public void Release(T element)
    {
        if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
            QLog.LogError("Internal error. Trying to destroy object that is already released to pool.");
        if (m_ActionOnRelease != null)
            m_ActionOnRelease(element);
        m_Stack.Push(element);
    }
}
```

ObjectPool代码来自UGUI源码。

下面我们看看StringPool的代码如何实现

```
/**
*   Author：onelei
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using System.Text;

public static class StringPool
{
    private static int MaxCount = 100;

    // Object pool to avoid allocations.
    private static readonly ObjectPool<StringBuilder> Pool = new ObjectPool<StringBuilder>(Clear, null);
    static void Clear(StringBuilder s)
    {
        if (Pool.countAll >= MaxCount)
        {
            QLog.LogError("Pool count reach to MaxCount.");
        }
        s.Remove(0, s.Length);
    }

    public static StringBuilder GetStringBuilder()
    {
        StringBuilder stringBuilder = Pool.Get();
        return stringBuilder;
    }

    public static void Release(StringBuilder toRelease)
    {
        if (Pool.countAll >= MaxCount)
        {
            QLog.LogError("Pool count reach to MaxCount.");
        }

        Pool.Release(toRelease);
    }

    public static string Concat(string s1, string s2)
    {
        StringBuilder stringBuilder = Pool.Get();
        stringBuilder.Append(s1);
        stringBuilder.Append(s2);
        string result = stringBuilder.ToString();
        Release(stringBuilder);
        return result;
    }

    public static string Concat(string s1, string s2, string s3)
    {
        StringBuilder stringBuilder = Pool.Get();
        stringBuilder.Append(s1);
        stringBuilder.Append(s2);
        stringBuilder.Append(s3);
        string result = stringBuilder.ToString();
        Release(stringBuilder);
        return result;
    }

    public static string Format(string src, params object[] args)
    {
        StringBuilder stringBuilder = Pool.Get();
        stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.AppendFormat(src, args);
        string result = stringBuilder.ToString();
        Release(stringBuilder);
        return result;
    }
}
```

我们在StringPool里面使用ObjectPool<StringBuilder>保存了一个Pool对象池。

```
// Object pool to avoid allocations.
    private static readonly ObjectPool<StringBuilder> Pool = new ObjectPool<StringBuilder>(Clear, null);
    static void Clear(StringBuilder s) { s.Remove(0, s.Length); }
```

同时在Pool的Get函数里面传入Clear函数，在获取到StringBuilder的时候执行Clear函数清除里面的数据。

接下来我们来看如何使用，其实和之前区别并不大，函数接口都没有改动，我们只需要改函数的内部实现即可，先看下Concat函数

```
    public static string Concat(string s1, string s2)
    {
        StringBuilder stringBuilder = Pool.Get();
        stringBuilder.Append(s1);
        stringBuilder.Append(s2);
        string result = stringBuilder.ToString();
        Release(stringBuilder);
        return result;
    }
```

首先Pool.Get()在缓存池里面获取一个StringBuilder变量，在操作好之后执行Release函数，将其放回池中。同理Format函数亦是如此。

我们注意到ObjectPool里面有这三个变量

```
    public int countAll { get; private set; }
    public int countActive { get { return countAll - countInactive; } }
    public int countInactive { get { return m_Stack.Count; } }
```

countAll是在每次new一个StringBuilder的时候自动加一，也就是池中的对象总数。

countInactive是在每次Release的时候放回m_Stack里面的，也就是被放回池中的，未使用对象。

剩下的countActive就是已经被利用的对象数量。

我们为了防止池没有被合理使用，我们可以设置一个池中的对象总数，作为代码防护，防止内存泄漏。大部分情况下是对象在用完之后，没有被及时执行Release函数，导致池中不断的new一个对象出来，因此我们可以在Get函数里面做一个安全判断，当超过池中最大容量的时候，抛出一个错误。

```
    static void Clear(StringBuilder s)
    {
        if (Pool.countAll >= MaxCount)
        {
            QLog.LogError("Pool count reach to MaxCount.");
        }
        s.Remove(0, s.Length);
    }
```

我们在Get的函数的回调函数Clear里面添加以上判断即可。

如果本文对你有所帮助，欢迎赞赏~~~



