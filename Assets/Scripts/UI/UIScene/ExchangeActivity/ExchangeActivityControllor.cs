using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 活动彩蛋
/// </summary>
public class ExchangeActivityControllor : UIBoxBase {
	
	public static ExchangeActivityControllor Instance = null;

	public EasyFontTextMesh datatimeText;
	public tk2dTextMesh eggCountText;
	public ExchangeActivityItem[] itemArray;

	public override void Init ()
	{
		Instance = this;
		SetTipsCount ();
		base.Init();
	}

	void OnEnable()
	{
		PlayerData.Instance.EggChangeEvent += UpdateEggCount;
	}

	void OnDisable()
	{
		PlayerData.Instance.EggChangeEvent -= UpdateEggCount;
	}

	public override void Show ()
	{
		base.Show ();
		InitData ();
		SetTipsCount ();

		ExchangeActivityData.Instance.InitFromServer ();
	}

	public override void Hide ()
	{
		base.Hide ();
		UIManager.Instance.HideModule (UISceneModuleType.ExchangeActivity);
	}

	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}

	public void CloseOnClick()
	{
		AudioManger.Instance.PlaySound (AudioManger.SoundName.CloseBtClick);
		Hide ();
	}


	public void InitData()
	{
		datatimeText.text = ExchangeActivityData.Instance.GetTimeString ();
		eggCountText.text = PlayerData.Instance.GetItemNum (PlayerData.ItemType.ColorEgg).ToString();
		List<ExchangeActivityData.DataStruct> itemList = ExchangeActivityData.Instance.GetItemList ();
		for (int i=0; i<itemArray.Length; ++i) {
			itemArray[i].SetData(itemList[i]);
		}
	}

	void UpdateEggCount(int count)
	{
		eggCountText.text = count.ToString ();
		SetTipsCount ();
	}

	void SetTipsCount()
	{
		int eggCount = PlayerData.Instance.GetItemNum (PlayerData.ItemType.ColorEgg);
		int count = 0;

		List<ExchangeActivityData.DataStruct> itemList = ExchangeActivityData.Instance.GetItemList ();
		for (int i=0; i<itemList.Count; ++i) {
			ExchangeActivityData.DataStruct item = itemList[i];
			if(eggCount > item.price && item.isGet== false && item.rest>0)
			{
				count ++;
			}
		}
		MainInterfaceControllor.Instance.SetExchangeActivityCount (count);
	}
	
}
