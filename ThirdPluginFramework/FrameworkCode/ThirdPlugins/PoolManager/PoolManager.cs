using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 对象池管理
/// 对象池相对路径为Resources/Pool，且不可更改
/// </summary>
public class PoolManager 
{
    /// <summary>
    /// 对象池的父节点
    /// </summary>
    private static Transform _poolNode = null;
    private static Transform PoolNode
    {
        get
        {
            if (_poolNode == null)
            {
                GameObject poolNode = new GameObject();
                Object.DontDestroyOnLoad(poolNode);
                poolNode.name = "PoolNode";
                poolNode.SetActive(true);
                _poolNode = poolNode.transform;
            }
            return _poolNode;
        }
    }
    /// <summary>
    /// 对象池用于存储对象的字典
    /// </summary>
    private static Dictionary<string, List<GameObject>> poolDict = new Dictionary<string, List<GameObject>>();
    /// <summary>
    /// 对象池用于存储预制体的字典
    /// </summary>
    private static Dictionary<string, GameObject> pfbDict = new Dictionary<string, GameObject>();
    /// <summary>
    /// 从对象池中取一个对象
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GameObject GetObjectFromPool(string name)
    {
        GameObject obj;
        if (poolDict.ContainsKey(name))
        {
            if (poolDict[name].Count == 0)
            {
                obj = InitObjectByName(name);
            }
            else
            {
                obj = poolDict[name][0];
                ///含有递归，注意正确使用对象池
                if(obj == null)
                {
                    poolDict[name].RemoveAt(0);
                    obj = GetObjectFromPool(name);
                }
                poolDict[name].RemoveAt(0);
            }
        }
        else
        {
            List<GameObject> poolList = new List<GameObject>();
            poolDict.Add(name, poolList);
            obj = InitObjectByName(name);
        }
        obj.SetActive(true);
        obj.GetComponent<IPoolBase>()?.OnEnabledByPool();
        return obj;
    }
    /// <summary>
    /// 通过名字初始化一个对象
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static GameObject InitObjectByName(string name)
    {
        GameObject pfb;
        if (pfbDict.ContainsKey(name))
        {
            pfb = pfbDict[name];
        }
        else
        {
            pfb = Resources.Load<GameObject>("Pool/" + name);
            pfbDict.Add(name, pfb);
        }
        GameObject obj = Object.Instantiate(pfb);
        obj.transform.SetParent(PoolNode,false);
        return obj;
    }
    /// <summary>
    /// 将对象从外部放入对象池
    /// </summary>
    /// <param name="name"></param>
    /// <param name="obj"></param>
    public static void PutObjectToPool(string name, GameObject obj)
    {
        obj.GetComponent<IPoolBase>()?.OnDisabledByPool();
        if (obj.transform.parent != PoolNode)
        {
            obj.transform.SetParent(PoolNode, false);
        }
        obj.gameObject.SetActive(false);
        if (poolDict.ContainsKey(name))
        {
            if (!poolDict[name].Contains(obj))
            {
                poolDict[name].Add(obj);
            }
        }
        else
        {
            List<GameObject> poolList = new List<GameObject>();
            poolList.Add(obj);
            poolDict.Add(name, poolList);
        }
    }
    /// <summary>
    /// 预缓存对象池
    /// </summary>
    /// <param name="name">对象名称</param>
    /// <param name="count">数量</param>
    public static void PreLoadedObject(string name, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = InitObjectByName(name);
            obj.SetActive(false);
            if (!poolDict.ContainsKey(name))
            {
                List<GameObject> poolList = new List<GameObject>();
                poolDict.Add(name, poolList);
            }
            poolDict[name].Add(obj);
        }
    }
}
