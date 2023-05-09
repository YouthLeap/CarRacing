using UnityEngine;
using System.Collections;
using PathologicalGames;

public class ClearanceRedPaper : UIBoxBase {

	public static ClearanceRedPaper Instance;

	public Transform ItemContainer;
	public tk2dUIScrollableArea scroll;
	
	private float ItemLength = 110;
	private float posY;
	private const int itemCount = 5;
	private Transform itemTran;
	private ClearanceRedPaperItem itemScript;

	private int _tipCount;
	public int TipCount
	{
		set
		{
			if(!PlatformSetting.Instance.isOpenClearanceRedPaper)
			{
				_tipCount = 0;
			}
			else
			{
				_tipCount = value;
			}

			if(_tipCount > 0)
			{
				ActivityControllor.Instance.ActivityItemList[3].TipsGameobj.SetActive(true);
			}
			else
			{
				ActivityControllor.Instance.ActivityItemList[3].TipsGameobj.SetActive(false);
			}
		}
		get{return _tipCount;}
	}
	public override void Init ()
	{
		Instance = this;
		TipCount = 0;
		if (!PlatformSetting.Instance.isOpenClearanceRedPaper)
			return;
		InitData ();
		SpawnPool spUIItems = PoolManager.Pools["UIItemsPool"];
		for(int i=0; i<itemCount; ++i)
		{
			itemTran = spUIItems.Spawn("ClearanceRedPaperItem");
			itemTran.parent = ItemContainer;
			itemTran.name = "ClearanceRePaperItem" + i;
			itemTran.localPosition = new Vector3(0, 100 - ItemLength * i, 0);
			itemTran.localScale = Vector3.one;
			itemScript = itemTran.GetComponent<ClearanceRedPaperItem> ();
			itemScript.Init (i + 1);
		}
		scroll.ContentLength = (itemCount - 2) * 80;

		base.Init();
	}

	void InitData()
	{
		if(!PlayerData.Instance.GetClearanceRedInit())
		{
			int[] tempIds = new int[itemCount];
			int[] tempState = new int[itemCount];
			int[] tempAlreadyIsGet = new int[itemCount];
			for(int i = 1; i <= itemCount; i++)
			{
				tempIds[i-1] = int.Parse(ClearanceRedPaperData.Instance.GetTargetLevel(i));
				tempState[i-1] = 0;
				tempAlreadyIsGet[i-1] = 0;
			}
			PlayerData.Instance.SetClearanceRedIds(tempIds);
			PlayerData.Instance.SetClearanceRedState(tempState);
			PlayerData.Instance.SetClearanceRedAlreadyIsGet(tempAlreadyIsGet);
			PlayerData.Instance.SetClearanceRedInit(true);
		}
	}

	public override void Show ()
	{
		base.Show ();
	}

	public override void Hide ()
	{
		base.Hide ();
		UIManager.Instance.HideModule (UISceneModuleType.ClearanceRedPaper);
	}

	public override void Back ()
	{
		base.Back ();
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}

	public void Close()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}
}
