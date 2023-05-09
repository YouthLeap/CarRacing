using UnityEngine;
using System.Collections;
using PathologicalGames;

public class LuckyNumbersController : UIBoxBase {

	public static LuckyNumbersController Instance = null;

	public Transform contentTran;
	private float ySpacing = 85;

	private int _tipCount;
	public int TipCount
	{
		set
		{
			if(!PlatformSetting.Instance.isOpenLuckyNumbersActivity)
			{
				_tipCount = 0;
			}
			else
			{
				_tipCount = value;
			}
			if(_tipCount > 0)
			{
				ActivityControllor.Instance.ActivityItemList[0].TipsGameobj.SetActive(true);
			}
			else
			{
				ActivityControllor.Instance.ActivityItemList[0].TipsGameobj.SetActive(false);
			}
		}
		get{return _tipCount;}
	}

	public override void Init ()
	{
		Instance = this;
		TipCount = 0;
		if (!PlatformSetting.Instance.isOpenLuckyNumbersActivity)
			return;
		SpawnPool spUIItems = PoolManager.Pools["UIItemsPool"];
		int itemCount = LuckyNumbersData.Instance.GetDataRow ();
		for(int i = 0; i < itemCount; i++)
		{
			Transform itemTran = spUIItems.Spawn("LuckyNumbersItem",contentTran);
			itemTran.localPosition = new Vector3(0,-i * ySpacing,0);
			LuckyNumbersItem itemScr = itemTran.GetComponent<LuckyNumbersItem>();
			itemScr.Init(i + 1);
		}
		base.Init();
	}
	public override void Show ()
	{
		base.Show ();
	}
	public override void Hide ()
	{
		base.Hide ();
		UIManager.Instance.HideModule (UISceneModuleType.LuckyNumbers);
	}
	public override void Back ()
	{
		Hide ();
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
	}
	public void Close()
	{
		Hide ();
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
	}
}
public enum LuckyNumbersType
{
	One,
	Six,
	Eight,
	OneSixEight
}
