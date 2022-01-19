using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
public class ConfigReader
{
    public delegate void NoParamCallback();
    /// <summary>
    /// 配置表读取完成后保存的实例,注：不要对其数据进行修改
    /// </summary>
    public static ConfigData configData = null;

    private static NoParamCallback _loadFinishCallback = null;
    /// <summary>
    /// 初始化并加载配置表
    /// </summary>
    /// <param name="loadFinishCallback">加载配置表完成后的回调</param>
    public static void LoadConfigData(NoParamCallback loadFinishCallback = null)
    {
        _loadFinishCallback = loadFinishCallback;
#if UNITY_WEBGL
        _loadFinishCallback?.Invoke();
        return;
#endif
        StartLoadConfig();
    }
    /// <summary>
    /// 开始加载
    /// </summary>
    private static void StartLoadConfig()
    {
        GameObject loadInstance = new GameObject();
        loadInstance.name = "LoadConfigInstance";
        loadInstance.AddComponent<ConfigReaderHelper>();
        GameObject.DontDestroyOnLoad(loadInstance);
    }
    private class ConfigReaderHelper : MonoBehaviour
    {
        UnityWebRequest request = null;
        private void Awake()
        {
            StartCoroutine(LoadConfig());
        }

        IEnumerator LoadConfig()
        {
            //for android
            string s = Application.streamingAssetsPath + "/ConfigJsons/game.bin";
#if UNITY_STANDALONE_OSX
        s = Application.dataPath + "/Resources/Data/StreamingAssets" + "/ConfigJsons/game.json";
#endif

#if UNITY_IOS
        s = "file://" + Application.dataPath + "/Raw/ConfigJsons/game.bin";
#endif

#if UNITY_EDITOR_OSX
        s = "file://" + Application.streamingAssetsPath + "/ConfigJsons/game.bin";
#endif

            request = UnityWebRequest.Get(s);
            request.SendWebRequest();
            yield return request;

            while (!request.isDone)
            {
                yield return new WaitForEndOfFrame();
            }

            if (request.isDone)
            {
                string str = request.downloadHandler.text;
                if(str == "")
                {
                    _loadFinishCallback?.Invoke();
                    yield break;
                }
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream(request.downloadHandler.data);
                configData = formatter.Deserialize(stream) as ConfigData;
                _loadFinishCallback?.Invoke();
            }
            yield return 0;
        }
    }
}
/// <summary>
/// 配置表数据类型
/// </summary>
[Serializable]
public class ConfigData
{
    public table_Unlock[] table_Unlock = null;
    public table_name[] table_name = null;
}
//以下自行创建配置表类,并添加到RootObject中
[Serializable]
public class table_Unlock
{
    public int Id;
    public int Unlocklevel;
}
[Serializable]
public class table_name
{
    public int Id;
    public string Name;
}