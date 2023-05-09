using UnityEngine;
using System.Collections;
using DG.Tweening;
using PathologicalGames;

public class ConvertCenterControllor : UIBoxBase {

	public static ConvertCenterControllor Instance;
	
	public GameObject goCloseButton;
	public Transform ItemContainer;
	public tk2dUIScrollableArea scroll;

	private int itemCount;
	private Transform itemTran;
	private ConvertItem itemScript;

	private float ItemLength = 165;

	public override void Init ()
	{
		Instance = this;
		InitData ();
		base.Init();
	}

	private void InitData()
	{
		SpawnPool spUIItems = PoolManager.Pools["UIItemsPool"];

		itemCount = ConvertCenterData.Instance.GetDataRow ();
		int fruitCount, totalCount;
		float posX;
		float width = itemCount * ItemLength;
		int convertCount = 0;
		for(int i=1; i<=itemCount; ++i)
		{
			itemTran = spUIItems.Spawn("ConvertItem");
			itemTran.gameObject.SetActive(true);
			itemTran.parent = ItemContainer;
			itemTran.name = "ConvertItem" + i;
			itemTran.localPosition = new Vector3(-165+ (ItemLength * (i - 1)), 0, 0);
			itemTran.localScale = Vector3.one;
			itemScript = itemTran.GetComponent<ConvertItem>();
			itemScript.Init(i);
			if(itemScript.convertFlag)
				++ convertCount;
		}
		scroll.ContentLength = (itemCount - 4) * ItemLength;

		MainInterfaceControllor.Instance.SetConvertCount (convertCount);
	}

	public void RefreshData()
	{
		int convertCount = 0;
		for(int i=1; i<=itemCount; ++i)
		{
			itemTran = ItemContainer.Find("ConvertItem" + i);
			itemScript = itemTran.GetComponent<ConvertItem>();
			itemScript.InitData ();
			if(itemScript.convertFlag)
				++ convertCount;
		}
		MainInterfaceControllor.Instance.SetConvertCount (convertCount);
	}
	
	public override void Show ()
	{
		base.Show();
		RefreshData ();
	}
	
	public override void Hide ()
	{
		base.Hide ();
		UIManager.Instance.HideModule (UISceneModuleType.ConvertCenter);
	}
	
	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		Hide();
	}

	void CloseOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}
	
}
