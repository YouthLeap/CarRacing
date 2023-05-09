using UnityEngine;
using System.Collections;
using DG.Tweening;
using PathologicalGames;
using PayBuild;

public class AotemanFamilyControllor : UIBoxBase {

	public GameObject goCloseButton, goBuyButton, goCancelBtn;
//	public Transform ItemContainer; //固定了图标位置，取消了ItemContainer
	public EasyFontTextMesh textHint1, textHint2, textPayBt;
	public tk2dSprite imageCloseBt;
	public EasyFontTextMesh textDesc1, textDesc2;

	public override void Init ()
	{
		//		InitData (); //固定了图标位置，取消了ItemContainer

		base.Init();
	}

/* 固定了图标位置，取消了ItemContainer
	private void InitData()
	{
		SpawnPool spUIItems = PoolManager.Pools["UIItemsPool"];
		Transform itemTran;
		AotemanItem itemScript;
		for(int i=2; i<6; ++i)
		{
			itemTran = spUIItems.Spawn("AotemanItem");
			itemTran.parent = ItemContainer;
			itemTran.localPosition = new Vector3(-200 + 135 * (i - 2), 0, 0);
			itemScript = itemTran.GetComponent<AotemanItem>();
			itemScript.SetIconImage(ModelData.Instance.GetPlayerIcon(i*100+1));
		}
	}
*/

	public override void Show ()
	{
		base.Show();

		PayUIConfigurator.PayUIConfig(PayType.CharactersGift, textHint1, textHint2, textDesc1, textDesc2, textPayBt, imageCloseBt, goBuyButton.GetComponent<BoxCollider>(), SetCancelBt);

		AudioManger.Instance.PlaySound(AudioManger.SoundName.AotemanXiongdi);

		//自定义事件.
		CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_AoteBrother, GiftEventType.Show);



		if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe) {
			goBuyButton.transform.Find("UI_libao01_hg").gameObject.SetActive(false); //审核版不显示按钮特效，更换特效时要改名称
		} 
	}

	void SetCancelBt(bool state)
	{
		if(state)
		{
			goCancelBtn.SetActive(true);
			goCancelBtn.transform.localPosition = new Vector3(-87, -170, 0);
			goBuyButton.transform.localPosition = new Vector3(114, -170, 0);
		}else
		{
			goCancelBtn.SetActive(false);
			goBuyButton.transform.localPosition = new Vector3(114, -170, 0);
		}
	}

	public override void Hide ()
	{
		AudioManger.Instance.StopSound(AudioManger.SoundName.AotemanXiongdi);
		base.Hide ();
		UIManager.Instance.HideModule (UISceneModuleType.AotemanFamily);
	}
	
	public override void Back ()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}

	private void PayCallBack(string result)
	{
		if (result.CompareTo ("Success") != 0)
			return;
		Hide();

		PlayerData.Instance.SetAoteBrotherState (true);

		bool hasModel = false;
		int[] modelState = PlayerData.Instance.GetModelState ();
		int modelCount = 5;
		for(int i=2; i<=modelCount; ++i)
		{
			hasModel = false;
			for(int j=0; j<modelState.Length; ++j)
			{
				if(IDTool.GetModelType(modelState[j]) == i)
				{
					hasModel = true;
					break;
				}
			}
			if(!hasModel)
			{
				PlayerData.Instance.UpdateModelState(i*100+1);
			}
		}

	    MainInterfaceControllor.Instance.goCharactersButton.SetActive(false);
	
		//自定义事件.
		CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_AoteBrother, GiftEventType.Pay);
	}

	void CloseOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide ();
	}

	void CancleOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
	}

	void BuyOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		PayBuildPayManage.Instance.Pay((int)PayType.CharactersGift, PayCallBack);
		//自定义事件.
		CollectInfoEvent.SendGiftEvent (CollectInfoEvent.EventType.Gift_AoteBrother, GiftEventType.Click);
	}
}
