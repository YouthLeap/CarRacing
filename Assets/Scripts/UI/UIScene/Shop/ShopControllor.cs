using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PathologicalGames;

public class ShopControllor : UIBoxBase {
	public static ShopControllor Instance;

	public Transform ItemContainer;

	private Transform ShopItemTran;
	private ShopItem ShopItemScript;
	private List<ShopItem> shopItemList = new List<ShopItem>();
	
	private int itemIndex;

	private float ItemLength = 140;
	private float FirstItemPosX = - 280;
	
	public override void Init()
	{
		Instance = this;

		SpawnPool spUIItems = PoolManager.Pools["UIItemsPool"];

		int itemCount = ShopData.Instance.GetDataRow ();
		float posX;
		shopItemList.Clear ();
		for(int i=1; i<=itemCount; ++i)
		{
			ShopItemTran = spUIItems.Spawn("ShopItem");
			ShopItemTran.parent = ItemContainer;
			ShopItemTran.name = "ShopItem" + i;
			posX = FirstItemPosX + ItemLength * (i - 1);
			ShopItemTran.localPosition = new Vector3(posX, 0, 0);
			ShopItemTran.localScale = Vector3.one;
			ShopItemScript = ShopItemTran.GetComponent<ShopItem> ();
			ShopItemScript.Init (i);
			shopItemList.Add (ShopItemScript);
		}

		base.Init();
	}

	public void CloseBtnOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide ();
	}

	public override void Hide ()
	{
		base.Hide ();
		if(GlobalConst.SceneName == SceneType.UIScene)
			UIManager.Instance.HideModule(UISceneModuleType.Shop);
		else
			GameUIManager.Instance.HideModule(UISceneModuleType.Shop);

		PropertyDisplayControllor.Instance.ChangeLayer ();
	}

	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide ();
	}
	
	public override void Show()
	{
		PropertyDisplayControllor.Instance.ChangeLayer ();
		base.Show ();

		//自定义事件.
		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.Shop, "Level", PlayerData.Instance.GetSelectedLevel ().ToString ());
	}

	/// <summary>
	/// 刷新双倍金币的显示
	/// </summary>
	public void RefreshDoubleCoinShow()
	{
		if (this == null || !gameObject.activeInHierarchy)
			return;

		for (int i = 0; i < shopItemList.Count; i++) {
			if (shopItemList [i].itemType == PlayerData.ItemType.DoubleCoin) {
				shopItemList [i].goBuyButton.GetComponent<BoxCollider>().enabled = false;
				shopItemList [i].SetBtnText ("Activated");
				break;
			}
		}
	}
}