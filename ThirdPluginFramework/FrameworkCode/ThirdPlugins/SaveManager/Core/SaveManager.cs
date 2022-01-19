using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Networking;
using UnityEngine;
#if CHANNEL_DOUYIN
using StarkSDKSpace;
#endif

/// <summary>
/// 存档管理器
/// </summary>
public class SaveManager
{
    public delegate void LoadDataFinishCallback();
    /// <summary>
    /// 类种类，用来存储存档数据
    /// </summary>
    [Serializable]
    private class SaveData
    {
        /// <summary>
        /// 存档数据列表
        /// </summary>
        public List<object> _saveList = new List<object>();
        /// <summary>
        /// 版本号
        /// </summary>
        public string _version = "";
    }
    /// <summary>
    /// 读档之后的调用
    /// </summary>
    private static LoadDataFinishCallback _loadFinishCallBack = null;
    /// <summary>
    /// 存档数据
    /// </summary>
    private static SaveData _saveData = null;
    /// <summary>
    /// 读档数据
    /// </summary>
    private static SaveData _loadData = null;

    private static GameObject autoSaveInstance = null;
    /// <summary>
    /// 初始化存档管理器，并读档
    /// </summary>
    /// <param name="callBack">读档完成之后调用的回调</param>
    public static void Init(LoadDataFinishCallback callBack = null)
    {
        _loadFinishCallBack = callBack;
        _saveData = new SaveData();
    }
    /// <summary>
    /// 注册需要存档的数据类
    /// </summary>
    /// <param name="data">数据类</param>
    public static void RegisterData(object data)
    {
        _saveData._saveList.Add(data);
    }
    /// <summary>
    /// 存储游戏
    /// </summary>
    public static void SaveGame()
    {
#if CHANNEL_DOUYIN
        StarkSDK.API.Save<SaveData>(_saveData);
        return;
#endif
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        }
        else
        {
            file = File.Create(Application.persistentDataPath + "/gamesave.save");
        }

        bf.Serialize(file, _saveData);
        file.Close();
    }
    /// <summary>
    /// 读取游戏存档数据
    /// </summary>
    public static void LoadUserData()
    {
#if UNITY_WEBGL && CHANNEL_DOUYIN
        _loadData = StarkSDK.API.LoadSaving<SaveData>();
        if (_loadData == null)
        {
            InitNewUserData();
        }
        else
        {
            SetUserData();
        }
        if (_loadFinishCallBack != null)
        {
            _loadFinishCallBack.Invoke();
            //开始自动存档
            if (autoSaveInstance == null)
            {
                InitAutoSaveSystem();
            }
        }
        return;
#endif
#if UNITY_ANDROID && CHANNEL_DOUYIN
        _loadData = StarkSDK.API.LoadSaving<SaveData>();
        if(_loadData == null)
        {
            if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
                try
                {
                    _loadData = ((SaveData)bf.Deserialize(file));
                    file.Close();
                    SetUserData();
                }
                catch
                {
                    InitNewUserData();
                }
            }
            else
            {
                InitNewUserData();
            }
        }
        else
        {
            SetUserData();
        }
        if (_loadFinishCallBack != null)
        {
            _loadFinishCallBack.Invoke();
            //开始自动存档
            if (autoSaveInstance == null)
            {
                InitAutoSaveSystem();
            }
        }
        return;
#endif

        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            try
            {
                _loadData = ((SaveData)bf.Deserialize(file));
                file.Close();
                SetUserData();
            }
            catch
            {
                InitNewUserData();
            }
        }
        else
        {
            InitNewUserData();
        }
        if (_loadFinishCallBack != null)
        {
            _loadFinishCallBack.Invoke();
            //开始自动存档
            if (autoSaveInstance == null)
            {
                InitAutoSaveSystem();
            }
        }
    }
    /// <summary>
    /// 复原存档
    /// </summary>
    private static void SetUserData()
    {
        List<object> data = _saveData._saveList;
        foreach (object item in data)
        {
            //根据类型获取读出数据的实例
            object getData = GetDataInstanceByType(item.GetType());
            if(getData == null)
            {
                continue;
            }
            //获取读出的数据的实例的属性
            FieldInfo[] propertys = getData.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            //遍历取出的所有的属性
            foreach (FieldInfo prop in propertys)
            {
                //根据取出属性名字，获取游戏中实例的属性
                FieldInfo info = item.GetType().GetField(prop.Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                if(info != null && prop.GetValue(getData) != null)
                {
                    info.SetValue(item, prop.GetValue(getData));
                }
            }
        }
    }
    /// <summary>
    /// 根据数据的类型，返回数据
    /// </summary>
    /// <param name="type">数据类型</param>
    /// <returns>返回的数据</returns>
    private static object GetDataInstanceByType(Type type)
    {
        for(int i = 0; i < _loadData._saveList.Count; i++)
        {
            if(type == _loadData._saveList[i].GetType())
            {
                return _loadData._saveList[i];
            }
        }
        return null;
    }
    /// <summary>
    /// 初始化新玩家的数据
    /// </summary>
    private static void InitNewUserData()
    {
        //初始化新玩家
        EventCenter.Broadcast(GameEventType.INIT_NEW_PLAYER);
    }

    /// <summary>
    /// 自动存档系统，默认设置每 5 秒自动存档
    /// </summary>
    private class AutoSaveSystem : MonoBehaviour
    {
        private float delayTime = 5;
        private IEnumerator AutoSaveGame()
        {
            while (true)
            {
                SaveGame();
                yield return new WaitForSeconds(delayTime);
            }
        }

        private void Start()
        {
            StartCoroutine(AutoSaveGame());
        }
    }
    /// <summary>
    /// 初始化自动存档系统
    /// </summary>
    private static void InitAutoSaveSystem()
    {
        GameObject save = new GameObject();
        save.AddComponent<AutoSaveSystem>();
        save.name = "SaveSystem";
        autoSaveInstance = save;
        GameObject.DontDestroyOnLoad(save);
    }

}
