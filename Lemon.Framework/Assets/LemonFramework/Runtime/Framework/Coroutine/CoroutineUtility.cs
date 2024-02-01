/**
*   Author：onelei
*   Coroutine tool, for using Coroutine easily.
*   Copyright © 2019 - 2020 ONELEI. All Rights Reserved
*/
using System;
using System.Collections.Generic;
using System.Collections;
using Lemon.Framework.Extension;
using Lemon.Framework.UI.Widgets;
using UnityEngine;

namespace Lemon.Framework
{
    public class CoroutineUtility : BaseBehavior
    {
        private static CoroutineUtility _instance;
        public static CoroutineUtility Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(CoroutineUtility).ToString());
                    _instance = go.GetOrAddComponent<CoroutineUtility>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        IEnumerator ICoroutine(float time = 0, bool ignoreTimeScale = false, Action callBack = null)
        {
            if (time == 0)
            {
                yield return null;
            }
            else if (time > 0)
            {
                if (ignoreTimeScale)
                {
                    float start = Time.realtimeSinceStartup;
                    while (Time.realtimeSinceStartup < start + time)
                    {
                        yield return null;
                    }
                }
                else
                    yield return new WaitForSeconds(time);
            }

            if (callBack != null)
            {
                callBack();
            }
        }

        /// <summary>
        /// 等待指定时间后，执行回调方法
        /// </summary>
        /// <param name="time">等待时间 秒，0为1帧</param>
        /// <param name="ignoreTimeScale">是否忽略TimeScale影响</param>
        /// <param name="callBack">回调方法</param>
        /// <param name="objects">回调参数</param>
        /// <returns></returns>
        public Coroutine WaitTime(float time = 0, bool ignoreTimeScale = false, Action callBack = null)
        {
            return StartCoroutine(ICoroutine(time, ignoreTimeScale, callBack));
        }



        /// <summary>
        /// 等待指定时间后，执行回调方法
        /// </summary>
        /// <param name="time">等待时间 秒，0为1帧</param>
        /// <param name="ignoreTimeScale">是否忽略TimeScale影响</param>
        /// <param name="callBack">回调方法</param>
        /// <param name="objects">回调参数</param>
        /// <returns></returns>
        public Coroutine WaitTime(float time = 0, bool ignoreTimeScale = false, Action<object[]> callBack = null, params object[] objects)
        {
            return StartCoroutine(ICoroutine(time, ignoreTimeScale, callBack, objects));
        }

        /// <summary>
        /// 倒计时或正计时 每隔指定时间回调一次
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="perTime">回调间隔</param>
        /// <param name="ignoreTimeScale">是否忽略TimeScale影响</param>
        /// <param name="callBack">回调方法</param>
        /// <returns></returns>
        public Coroutine WaitPerSecond(float startTime = 10, float endTime = 0, float perTime = 1, bool ignoreTimeScale = false, Action<float> callBack = null)
        {
            return StartCoroutine(ICoroutine(startTime, endTime, perTime, ignoreTimeScale, callBack));
        }

        /// <summary>
        /// 等待FixedUpdate时间后执行回调
        /// </summary>
        /// <param name="time">等待时间 秒，0为1帧</param>
        /// <param name="callBack">回调方法</param>
        /// <param name="objects">回调参数</param>
        /// <returns></returns>
        public Coroutine WaitFixedUpdate(float time = 0, Action<object[]> callBack = null, params object[] objects)
        {
            return StartCoroutine(ICoroutine(time, callBack, objects));
        }

        /// <summary>
        /// 主动停止指定协程
        /// </summary>
        /// <param name="coroutine"></param>
        public void Stop(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        IEnumerator ICoroutine(float time = 0, bool ignoreTimeScale = false, Action<object[]> callBack = null, params object[] objects)
        {
            if (time == 0)
            {
                yield return null;
            }
            else if (time > 0)
            {
                if (ignoreTimeScale)
                {
                    float start = Time.realtimeSinceStartup;
                    while (Time.realtimeSinceStartup < start + time)
                    {
                        yield return null;
                    }
                }
                else
                    yield return new WaitForSeconds(time);
            }

            if (callBack != null)
            {
                callBack(objects);
            }
        }


        IEnumerator ICoroutine(float beginTime = 10, float endTime = 0, float perTime = 1, bool ignoreTimeScale = false, Action<float> callBack = null)
        {
            if (beginTime > endTime)
            {
                while (beginTime > endTime)
                {
                    if (ignoreTimeScale)
                    {
                        float start = Time.realtimeSinceStartup;
                        while (Time.realtimeSinceStartup < start + perTime)
                        {
                            yield return null;
                        }
                    }
                    else
                        yield return new WaitForSeconds(perTime);
                    beginTime -= perTime;
                    if (callBack != null)
                    {
                        callBack(beginTime);
                    }
                }
            }
            else if (beginTime < endTime)
            {
                while (beginTime < endTime)
                {
                    if (ignoreTimeScale)
                    {
                        float start = Time.realtimeSinceStartup;
                        while (Time.realtimeSinceStartup < start + perTime)
                        {
                            yield return null;
                        }
                    }
                    else
                        yield return new WaitForSeconds(perTime);
                    beginTime += perTime;
                    if (callBack != null)
                    {
                        callBack(beginTime);
                    }
                }
            }

        }

        IEnumerator ICoroutine(float waitTime, Action<object[]> callBack = null, params object[] objects)
        {
            if (waitTime == 0)
            {
                yield return new WaitForFixedUpdate();
            }
            else if (waitTime > 0)
            {
                float time = 0;
                while (time < waitTime)
                {
                    yield return new WaitForFixedUpdate();
                    time += Time.fixedDeltaTime;
                }
            }

            if (callBack != null)
            {
                callBack(objects);
            }
        }
    }
}