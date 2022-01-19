using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageTips : UIBase
{

    private Text tipText;

    /// <summary>
    /// 初始化提示信息
    /// </summary>
    /// <param name="tips"></param>
    public void InitTips(string tips)
    {
        tipText.text = tips;
    }
    public void DestroyMine()
    {
        Destroy(gameObject);
    }

    private IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
    
    private void FindProperty()
    {
        tipText = transform.Find("TipsBg/Tips").GetComponent<Text>();
    }
    private void Awake()
    {
        //关闭上一个Messagetips
        EventCenter.Broadcast(GameEventType.REMOVE_LAST_MESSAGETIPS);
        FindProperty();
        EventCenter.AddListener(GameEventType.REMOVE_LAST_MESSAGETIPS, DestroyMine);
    }
    private void Start()
    {
	
	}

	private void Update()
    {
	
	}

	/// <summary>
    ///	OnUIDestroy方法必须在OnDestroy中调用 
    /// </summary>
	protected override void OnDestroy()
	{
		base.OnDestroy();
        EventCenter.RemoveListener(GameEventType.REMOVE_LAST_MESSAGETIPS, DestroyMine);
    }
}