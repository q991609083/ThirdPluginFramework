using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    /// <summary>
    /// 音频源文件目录，前置目录为Resources
    /// </summary>
    private static string clipPath = "Audios/";
    /// <summary>
    /// 播放音频的组件
    /// </summary>
    private static AudioSource source = null;
    /// <summary>
    /// AudioClip存储的字典，防止重复加载耗费性能
    /// </summary>
    private static Dictionary<string, AudioClip> clipDict = new Dictionary<string, AudioClip>();
    /// <summary>
    /// 存储OneShot的上次播放时间的字典(存储单位为毫秒)
    /// </summary>
    private static Dictionary<string, long> lastPlayTsDict = new Dictionary<string, long>();
    /// <summary>
    /// 存储OneShot的音频长度的字典，用于防止多个同音频同时播放
    /// </summary>
    private static Dictionary<string, float> audioTimeDict = new Dictionary<string, float>();
    /// <summary>
    /// 用来存储循环播放的音频的字典
    /// </summary>
    private static Dictionary<string, AudioSource> loopAudioDict = new Dictionary<string, AudioSource>();
    /// <summary>
    /// 同音频播放的间隔时间，单位为毫秒
    /// </summary>
    private static float compareTime = 200;

    #region 初始化AudioSource
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitAudioManager()
    {
        GameObject audioManager = new GameObject();
        audioManager.name = "AudioManager";
        Object.DontDestroyOnLoad(audioManager);
        source = audioManager.AddComponent<AudioSource>();
    }
    #endregion

    public static void Play(string name, bool loop = false,float volume = 1.0f)
    {
        ///如果不为Loop，则为OneShot，进行数据记录以及同音频播放对比
        if (!loop)
        {
            long ts = GameTools.GetTimeStampByMillisecond();
            if (lastPlayTsDict.ContainsKey(name))
            {
                if(ts - lastPlayTsDict[name] <= audioTimeDict[name] * compareTime)
                {
                    return;
                }
                else
                {
                    lastPlayTsDict[name] = ts;
                }
            }
            else
            {
                lastPlayTsDict.Add(name, ts);
            }
        }

        if (!clipDict.ContainsKey(name))
        {
            clipDict.Add(name, Resources.Load<AudioClip>(clipPath + name));
        }

        AudioClip clip = clipDict[name];
        if (!audioTimeDict.ContainsKey(name))
        {
            audioTimeDict.Add(name, clip.length);
        }
        if (!loop)
        {
            source.PlayOneShot(clip, volume);
        }
        else
        {
            if (!loopAudioDict.ContainsKey(name))
            {
                GameObject obj = new GameObject(name);
                obj.transform.SetParent(source.transform);
                AudioSource audio = obj.AddComponent<AudioSource>();
                audio.playOnAwake = false;
                audio.volume = volume;
                audio.loop = true;
                audio.clip = clip;
                audio.priority = 100;
                audio.Play();
                loopAudioDict[name] = audio;
            }
            else
            {
                if (!loopAudioDict[name].isPlaying)
                {
                    loopAudioDict[name].Play();
                }
            }
        }
    }
    /// <summary>
    /// 停止一个循环播放的音效
    /// </summary>
    /// <param name="name"></param>
    public static void Stop(string name)
    {
        if (source.transform.Find(name) != null)
        {
            if (source.transform.Find(name).GetComponent<AudioSource>().isPlaying)
            {
                source.transform.Find(name).GetComponent<AudioSource>().Stop();
            }
        }
    }
    /// <summary>
    /// 停止所有循环播放的音效
    /// </summary>
    public static void StopAll()
    {
        foreach (AudioSource audio in loopAudioDict.Values)
        {
            audio.Stop();
        }
    }
}
