using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
/// <summary>
/// UI名字通过枚举方式展现，防止UI名字打错
/// </summary>
#region UI枚举
public enum UIName
{
    LoadingUI,
    SettlementUI,
    GameUI,
    
    
    MainUI,
    StoreUI,
    TryStoreUI,
    MessageTips,
    SettleSuccessUI,
    SettleFailUI,
    FollowUI,
    RollUI,
    EpicSkinUI,
    WaitingUI,
    RecordUI,
    CrazyControllUI,
    BoxAwardUI,
    GiftUI,
    SignUI,
    PlayerCreateUI,
    UnlockBossUI,
    BossFightUI,
    SettingUI,
    TwelveOlderUI,
    StarStageUI,
    UnlockWeaponUI,
}
#endregion
public class UIManager : MonoBehaviour
{
    public delegate Vector2 WorldPosToCanvasPosDelegate(Transform tran);

    public static WorldPosToCanvasPosDelegate WorldPosToCanvasPos;

    public Camera worldCamera;
    /// <summary>
    /// UI界面的字典
    /// </summary>
    private Dictionary<UIName, GameObject> _uiPrefabDic = new Dictionary<UIName, GameObject>();

    private List<UIBase> _uiQueue = new List<UIBase>();
    /// <summary>
    /// 预加载所有枚举项对应的UI
    /// </summary>
    private void PreloadAllUI()
    {
        foreach (var item in Enum.GetNames(typeof(UIName)))
        {
            UIName key = (UIName)Enum.Parse(typeof(UIName),item);
            if (!_uiPrefabDic.ContainsKey(key))
            {
                GameObject pfb = Resources.Load<GameObject>("UI/" + item);
                _uiPrefabDic.Add(key, pfb);
            }
        }
    }
    /// <summary>
    /// 使用队列打开UI
    /// </summary>
    /// <param name="uiName">要打开的UI的名字</param>
    private void OpenUIUseQueue(UIName uiName)
    {
        if (!_uiPrefabDic.ContainsKey(uiName))
        {
            GameObject pfb = Resources.Load<GameObject>("UI/" + uiName.ToString());
            _uiPrefabDic.Add(uiName, pfb);
        }
        GameObject ui = Instantiate(_uiPrefabDic[uiName]);
        if(_uiQueue.Count == 0)
        {
            ui.transform.SetParent(transform,false);
        }
        else
        {
            ui.SetActive(false);
        }
#if UNITY_WEBGL
        ChangeFont(ui);
#endif
        UIBase uiBase = ui.GetComponent<UIBase>();
        if (uiBase != null)
        {
            uiBase.SetQueue(true);
            _uiQueue.Add(uiBase);
        }
    }
    /// <summary>
    /// 不使用队列打开UI
    /// </summary>
    /// <param name="uiName"></param>
    private void OpenUIUnuseQueue(UIName uiName)
    {
        if (!_uiPrefabDic.ContainsKey(uiName))
        {
            GameObject pfb = Resources.Load<GameObject>("UI/" + uiName.ToString());
            _uiPrefabDic.Add(uiName, pfb);
        }
        GameObject ui = Instantiate(_uiPrefabDic[uiName]);
        ui.transform.SetParent(transform, false);
#if UNITY_WEBGL
        ChangeFont(ui);
#endif
        ui.transform.SetSiblingIndex(999);
    }

    public static void ChangeFont(GameObject ui)
    {
        Font font = Resources.Load<Font>("Font/simhei");
        Text[] texts = ui.transform.GetComponentsInChildren<Text>(true);
        foreach (var text in texts)
        {
            text.font = font;
        }
    }
    /// <summary>
    /// 检测UI队列并打开下一个UI
    /// </summary>
    /// <param name="uiBase"></param>
    private void CheckQueueAndOpenNextUI(UIBase uiBase)
    {
        if (_uiQueue.Contains(uiBase))
        {
            _uiQueue.Remove(uiBase);
        }
        if(_uiQueue.Count != 0)
        {
            UIBase nextUI = _uiQueue[0];
            nextUI.transform.SetParent(transform, false);
            nextUI.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 世界坐标转Canvas坐标
    /// </summary>
    /// <param name="tran"></param>
    /// <returns></returns>
    private Vector2 WorldPosToCanvasPosTool(Transform tran)
    {
        Vector2 canvasSize = GetComponent<RectTransform>().sizeDelta;
        Vector3 viewPortPos3d;
        if(worldCamera == null)
        {
           viewPortPos3d = Camera.main.WorldToViewportPoint(tran.position);
        }
        else
        {
            viewPortPos3d = worldCamera.WorldToViewportPoint(tran.position);
        }
        Vector2 viewPortRelative = new Vector2(viewPortPos3d.x - 0.5f, viewPortPos3d.y - 0.5f);
        Vector2 cubeScreenPos = new Vector2(viewPortRelative.x * canvasSize.x, viewPortRelative.y * canvasSize.y);
        return cubeScreenPos;
    }
    private void MessageTips(string tip)
    {
        if (!_uiPrefabDic.ContainsKey(UIName.MessageTips))
        {
            GameObject pfb = Resources.Load<GameObject>("UI/MessageTips");
            _uiPrefabDic.Add(UIName.MessageTips, pfb);
        }
        GameObject ui = Instantiate(_uiPrefabDic[UIName.MessageTips]);
        ui.transform.SetParent(transform, false);
        ui.transform.SetSiblingIndex(100);
#if UNITY_WEBGL
        ChangeFont(ui);
#endif
        ui.GetComponent<MessageTips>().InitTips(tip);
    }
    private void Awake()
    {
        EventCenter.AddListener(GameEventType.PRELOAD_ALL_UI, PreloadAllUI);
        EventCenter.AddListener<string>(GameEventType.MESSAGETIPS, MessageTips);
        EventCenter.AddListener<UIName>(GameEventType.OPEN_UI_USE_QUEUE, OpenUIUseQueue);
        EventCenter.AddListener<UIName>(GameEventType.OPEN_UI_UNUSE_QUEUE, OpenUIUnuseQueue);
        EventCenter.AddListener<UIBase>(GameEventType.CHECK_QUEUE_AND_OPEN_NEXT_UI, CheckQueueAndOpenNextUI);
        WorldPosToCanvasPos += WorldPosToCanvasPosTool;
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(GameEventType.PRELOAD_ALL_UI, PreloadAllUI);
        EventCenter.RemoveListener<string>(GameEventType.MESSAGETIPS, MessageTips);
        EventCenter.RemoveListener<UIName>(GameEventType.OPEN_UI_USE_QUEUE, OpenUIUseQueue);
        EventCenter.RemoveListener<UIName>(GameEventType.OPEN_UI_UNUSE_QUEUE, OpenUIUnuseQueue);
        EventCenter.RemoveListener<UIBase>(GameEventType.CHECK_QUEUE_AND_OPEN_NEXT_UI, CheckQueueAndOpenNextUI);
        WorldPosToCanvasPos -= WorldPosToCanvasPosTool;
    }
}

