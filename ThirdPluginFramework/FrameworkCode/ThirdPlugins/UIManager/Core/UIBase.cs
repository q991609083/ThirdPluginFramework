using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIBase : MonoBehaviour
{
    /// <summary>
    /// 打开此UI时是否使用队列
    /// </summary>
    protected bool _useQueue = false;
    /// <summary>
    /// 设置是否使用队列标志位
    /// </summary>
    public void SetQueue(bool state)
    {
        _useQueue = state;
    }

    /// <summary>
    /// 当UI被销毁时，必须调用此方法 
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (_useQueue)
        {
            EventCenter.Broadcast(GameEventType.CHECK_QUEUE_AND_OPEN_NEXT_UI,this);
        }
    }

    protected void AlignLayout(string ctrlPath)
    {
        var layout = transform.Find(ctrlPath).GetComponent<VerticalLayoutGroup>();
#if CHANNEL_HUAWEI
        layout.enabled = true;
        layout.childAlignment = TextAnchor.UpperCenter;
        ThirdSdk.ThirdAdInter.PlayNativeAd();
// #else
        // layout.childAlignment = TextAnchor.MiddleCenter;
#endif
    }

    protected void HandleHideNativeAd()
    {
#if CHANNEL_XIAOMI
            ThirdSdk.ThirdAdInter.HideNativeAd();
#endif
    }

    protected virtual void Start()
    {
        CheckTranslate();
    }

    protected void CheckTranslate()
    {
        StartCoroutine(DelayTranslate());
    }

    protected IEnumerator DelayTranslate()
    {
        while (true)
        {
            if (LanguageSetting.LanguageManager.Instance.languageType == LanguageSetting.LanguageType.English)
            {
                Text[] textGroup = transform.GetComponentsInChildren<Text>(true);
                foreach (var text in textGroup)
                {
                    string lastStr = text.text;
                    if (LanguageSetting.LanguageData.textTranslateData.ContainsKey(lastStr))
                    {
                        string newStr = LanguageSetting.LanguageData.textTranslateData[lastStr][(int)LanguageSetting.LanguageManager.Instance.languageType - 1];
                        text.text = newStr;
                    }
                }
                Image[] imageGroup = transform.GetComponentsInChildren<Image>(true);
                foreach (var image in imageGroup)
                {
                    if (image.sprite == null) continue;
                    string imageName = image.sprite.name;
                    if (LanguageSetting.LanguageData.imageTranslateData.Contains(imageName))
                    {
                        Sprite newSprite = Resources.Load<Sprite>("Images" + (int)(LanguageSetting.LanguageManager.Instance.languageType) + "/" + imageName);
                        image.sprite = newSprite;
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
