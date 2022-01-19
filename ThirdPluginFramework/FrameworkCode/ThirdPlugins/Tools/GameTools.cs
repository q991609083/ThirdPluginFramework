using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void NoParamCallback();
public static class GameTools 
{
    /// <summary>
    /// 销毁此Transform下所有的节点
    /// </summary>
    /// <param name="transform"></param>
    public static void DestroyAllChildren(this Transform transform)
    {
        List<Transform> childList = new List<Transform>();
        int childCount = transform.childCount;
        for(int i = 0; i < childCount; i++)
        {
            childList.Add(transform.GetChild(i));
        }
        for(int i = childList.Count - 1; i >= 0; i--)
        {
            GameObject.Destroy(childList[i].gameObject);
        }
    }
    /// <summary>
    /// 向量旋转
    /// </summary>
    /// <param name="source"></param>
    /// <param name="axis"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 Rotate(this Vector3 source, Vector3 axis, float angle)
    {
        Quaternion q = Quaternion.AngleAxis(angle, axis);// 旋转系数
        return q * source;// 返回目标点
    }

    public static Quaternion ClampY(this Quaternion q, float min, float max)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;
        float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
        angleY = Mathf.Clamp(angleY, min, max);
        q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);
        return q;
    }
    /// <summary>
    /// 获取时间戳，秒为单位
    /// </summary>
    /// <returns></returns>
    public static long GetTimeStampBySecond()
    {
        TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
    /// <summary>
    /// 获取时间戳，毫秒为单位
    /// </summary>
    /// <returns></returns>
    public static long GetTimeStampByMillisecond()
    {
        TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalMilliseconds);
    }
    /// <summary>
    /// 获取今日0点的时间戳
    /// </summary>
    /// <returns></returns>
    public static long GetTodayZeroTime()
    {
        TimeSpan ts = DateTime.Now.Date - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
    /// <summary>
    /// 获取今日24点的时间戳
    /// </summary>
    /// <returns></returns>
    public static long GetTodayEndTime()
    {
        return GetTodayZeroTime() + 24 * 60 * 60;
    }
    /// <summary>
    /// 检查此时的时间戳与给定的时间戳是否为跨天
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns></returns>
    public static bool CheckIsNextDay(long lastTimeStamp)
    {
        if (lastTimeStamp < GetTodayZeroTime())
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 给定的物体的包围盒
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    public static Vector3 GetTargetSize(Transform trans)
    {
        Vector3 center = Vector3.zero;
        Renderer[] renderers = trans.GetComponentsInChildren<Renderer>();
        foreach (Renderer child in renderers)
        {
            center += child.bounds.center;
        }
        center /= renderers.Length;
        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach (Renderer item in renderers)
        {
            bounds.Encapsulate(item.bounds);
        }
        return bounds.size;
    }

    public static void ListRandom<T>(List<T> sources)
    {
        System.Random rd = new System.Random();
        int index = 0;
        T temp;
        for (int i = 0; i < sources.Count; i++)
        {
            index = rd.Next(0, sources.Count - 1);
            if (index != i)
            {
                temp = sources[i];
                sources[i] = sources[index];
                sources[index] = temp;
            }
        }
    }

    public static List<T> GetSubList<T>(List<T> targetList, List<T> totalList)
    {
        List<T> resultList = new List<T>();
        foreach (var item in totalList)
        {
            if (!targetList.Contains(item))
            {
                resultList.Add(item);
            }
        }
        return resultList;
    }

    public static List<T> GetMixedList<T>(this List<T> targetList, List<T> totalList)
    {
        List<T> resultList = new List<T>();
        foreach (var item in totalList)
        {
            if (targetList.Contains(item))
            {
                resultList.Add(item);
            }
        }
        return resultList;
    }
}
