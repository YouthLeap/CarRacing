using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ActivityControllor : UIBoxBase {
	
	public static ActivityControllor Instance = null;
	
	public List<ActivityItem> ActivityItemList = new List<ActivityItem> ();
	public GameObject goCloseButton;
	public Transform ItemContainer;
	
	private int _tipCount;
	public int TipCount
	{
		set
		{
			_tipCount = value;
			if(_tipCount > 0)
			{
				MainInterfaceControllor.Instance.ActivityTips.SetActive(true);
			}
			else
			{
				MainInterfaceControllor.Instance.ActivityTips.SetActive(false);
			}
		}
		get{return _tipCount;}
	}
	
	public override void Init ()
	{
		Instance = this;
		TipCount = 0;
		InitData ();

		base.Init();
	}
	
	private void InitData()
	{
		Transform itemTran;
		ActivityItem itemScript;
		for (int i=1; i<=4; ++i) {
			itemTran = ItemContainer.Find("ActivityItem" + i);
			itemScript = itemTran.GetComponent<ActivityItem>();
			itemScript.Init(i);
			ActivityItemList.Add(itemScript);
		}
	}
	
	public override void Show ()
	{
		base.Show ();
		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.Activity, "Level", PlayerData.Instance.GetSelectedLevel ().ToString ());
	}
	
	public override void Hide ()
	{
		base.Hide ();
		UIManager.Instance.HideModule (UISceneModuleType.Activity);
	}
	
	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}
	
	void CloseOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
		
	}	
}