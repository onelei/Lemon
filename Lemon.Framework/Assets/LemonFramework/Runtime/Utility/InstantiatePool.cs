/**
*   Author：onelei
*   https://github.com/onelei/LemonFramework
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class InstantiatePool
{
    public static void Instantiate<T>(Transform parent, T template, int needCount, ref List<T> list) where T : Component
    {
        int currentCount = list.Count;
        if (currentCount < needCount)
        {
            int moreCount = needCount - currentCount;
            for (int i = 0; i < moreCount; i++)
            {
                T item = GameObject.Instantiate<T>(template, parent, false);
                list.Add(item);
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            T item = list[i];
            bool bActive = (i < needCount);
            item.gameObject.SetActive(bActive);
        }
        template.gameObject.SetActive(false);
    }

}
