using UnityEngine;
using System.Collections;
using DG.Tweening;
using PathologicalGames;

public class StrengthControllor : UIBoxBase {
	public static StrengthControllor Instance;
	public GameObject goCloseButton;
	public Transform ItemContainer;
	
	public override void Init ()
	{
		Instance = this;
		InitData ();

		base.Init();
	}

	private void InitData()
	{
		SpawnPool spUIItems = PoolManager.Pools["UIItemsPool"];
		Transform itemTran;
		StrengthItem itemScript;
		for(int i=1; i<=3; ++i)
		{
			itemTran = spUIItems.Spawn("StrengthItem");
			itemTran.parent = ItemContainer;
			itemTran.localPosition = new Vector3(0, 60 - 102 * (i - 1), 0);
			itemTran.localScale = Vector3.one;
			itemScript = itemTran.GetComponent<StrengthItem>();
			itemScript.Init();
			itemScript.iItemID = i;
			itemScript.SetCostCountText(StrengthData.Instance.GetCostCount(i));
			itemScript.SetCountText(StrengthData.Instance.GetCount(i));
			itemScript.SetCostTypeImage(StrengthData.Instance.GetTypeName(i));
			itemScript.SetItemType(StrengthData.Instance.GetItemType(i));
		}
	}
	
	public override void Show ()
	{
		base.Show ();
	}
	
	public override void Hide ()
	{
		base.Hide ();
		if(GlobalConst.SceneName == SceneType.UIScene)
			UIManager.Instance.HideModule (UISceneModuleType.Strength);
		else
			GameUIManager.Instance.HideModule (UISceneModuleType.Strength);
	}
	
	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}
	
	public void CloseButtonOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
		return;
	}
	
}
