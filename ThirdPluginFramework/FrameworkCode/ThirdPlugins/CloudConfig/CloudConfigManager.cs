using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

/// <summary>
/// 自定义云控字段
/// </summary>

public class CloudConfigData
{
    public string onlineVersion; // 版本号
    /// <summary>
    /// 是否开启关注公众号按钮 1开启  0关闭
    /// </summary>
    public int followBtn = 0;
    /// <summary>
    /// 分享标题
    /// </summary>
    public string shareTitle = "小兵突突突，打你没商量";
    public string aboutText = "关于我们";
    /// <summary>
    /// 分享话题
    /// </summary>
    public List<string> topics = new List<string>() { "打你没商量" };
    /// <summary>
    /// 图文(半屏)插屏广告的间隔时间
    /// </summary>
    public int interDelayTime = 50;
    /// <summary>
    /// 全屏视频插屏广告的间隔时间
    /// </summary>
    public int fullscreenDelayTime = 50;

    public int showNativeAd = 0; //是否展示原生广告
}

public class CloudConfigManager 
{
    /// <summary>
    /// 云控数据
    /// </summary>
    public static CloudConfigData configData = null;
    /// <summary>
    /// 
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void InitCloudConfig()
    {
        GameObject obj = new GameObject();
        obj.AddComponent<LoadCloudConfigPlugin>();
        Object.DontDestroyOnLoad(obj);
    }

    /// <summary>
    /// 启动加载云控插件
    /// </summary>
    private class LoadCloudConfigPlugin : MonoBehaviour
    {
        private string configUrl = "https://qcdn.52wanh5.com/chaoxiuxian/TheyAreComing/Douyin.conf";
        /// <summary>
        /// 加载云控协程
        /// </summary>
        private Coroutine loadConfigCoroutine = null;
        /// <summary>
        /// 超时重试协程
        /// </summary>
        private Coroutine timeOutHandleCoroutine = null;
        /// <summary>
        /// 超时重试的次数
        /// </summary>
        private int repeatTimes = 9999;
        
        private void Awake()
        {
#if UNITY_EDITOR
            // configUrl = "http://192.168.0.33/config/TheyAreComing.conf";
#endif

#if CHANNEL_OPPO
        configUrl = "https://qcdn.52wanh5.com/chaoxiuxian/TheyAreComing/Douyin_oppo.conf";
#elif CHANNEL_XIAOMI
        configUrl = "https://qcdn.52wanh5.com/chaoxiuxian/TheyAreComing/Douyin_xiaomi.conf";
#endif
            configData = new CloudConfigData();
            if(configUrl == "")
            {
                Debug.LogError("云控地址为空，使用默认云控");
                return;
            }
            loadConfigCoroutine = StartCoroutine(LoadCloudConfig());
            timeOutHandleCoroutine = StartCoroutine(TimeOutHandle());
        }

        /// <summary>
        /// 协程，用于获取云控
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadCloudConfig()
        {
            UnityWebRequest loadW = UnityWebRequest.Get(configUrl);
            yield return loadW.SendWebRequest();
            while (loadW.isDone == false)
            {
                yield return new WaitForEndOfFrame();
            }
            if (loadW.isDone)
            {
                if (loadW.error == null)
                {
                    string str = loadW.downloadHandler.text;
                    JSONClass node = JSONNode.Parse(str) as JSONClass;
                    JSONArray jsArray = node["configData"] as JSONArray;
                    List<CloudConfigData> list = new List<CloudConfigData>();
                    foreach (JSONClass json in jsArray)
                    {
                        CloudConfigData data = new CloudConfigData();
                        SetConfigValue(data, json);
                        list.Add(data);
                    }
                    configData = list[0];
                    Debug.Log("云控加载成功");
                    OnLoadFinish();
                }
            }
        }
        /// <summary>
        /// 超时重试的协程
        /// </summary>
        /// <returns></returns>
        IEnumerator TimeOutHandle()
        {
            long ts = GameTools.GetTimeStampBySecond();
            while (true)
            {
                yield return 1;
                if (GameTools.GetTimeStampBySecond() - ts >= 2)
                {
                    if(loadConfigCoroutine != null)
                    {
                        ///关闭上一个加载协程
                        StopCoroutine(loadConfigCoroutine);
                        ///延迟0.2秒（减缓网络通道压力）
                        yield return new WaitForSeconds(0.2f);
                        ///重新开始云控拉取流程
                        loadConfigCoroutine = StartCoroutine(LoadCloudConfig());
                        ts = GameTools.GetTimeStampBySecond();
                        repeatTimes -= 1;

                        ///超时重试次数用完，停止协程
                        if (repeatTimes <= 0)
                        {
                            timeOutHandleCoroutine = null;
                            yield break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据自己的配置，修改配置参数
        /// </summary>
        /// <param name="data">使用的数据</param>
        /// <param name="json">云控拉取的字符串</param>
        private void SetConfigValue(CloudConfigData data,JSONClass json)
        {            
            data.interDelayTime = json["interDelayTime"].AsInt == 0?50: json["interDelayTime"].AsInt;
            data.fullscreenDelayTime = json["fullscreenDelayTime"].AsInt == 0 ? 50 : json["fullscreenDelayTime"].AsInt;

            data.onlineVersion = json["onlineVersion"];
            
                
            if (json["aboutText"].ToString() != "")
            {
                data.aboutText = json["aboutText"];
            }

            if (json["topics"] != null && json["topics"] != "")
            {
                string topicsStr = json["topics"];
                string[] strArray = topicsStr.Split('&');
                if (strArray.Length != 0)
                {
                    data.topics = strArray.ToList();
                }
            }
            if (json["shareTitle"] != null && json["shareTitle"] != "")
            {
                data.shareTitle = json["shareTitle"];
            }

            if (json["showNativeAd"] != null)
            {
                data.showNativeAd = json["showNativeAd"].AsInt == 0 ? 0 : 1;
            }
        }
        /// <summary>
        /// 云控加载成功，销毁加载介质
        /// </summary>
        private void OnLoadFinish()
        {
            if (timeOutHandleCoroutine != null)
            {
                StopCoroutine(timeOutHandleCoroutine);
            }
            Destroy(gameObject);
        }
    }
}

