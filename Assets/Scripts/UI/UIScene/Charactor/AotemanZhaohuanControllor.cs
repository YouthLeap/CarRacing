using UnityEngine;
using System.Collections;
using DG.Tweening;

public class AotemanZhaohuanControllor : UIBoxBase {

	public static AotemanZhaohuanControllor Instance;

	public GameObject goCloseButton, goZhaohuanButton;

	public tk2dSprite imageAoteIcon, imageCostType, roleSprite;
	public tk2dSprite imageCloseBt;
	public EasyFontTextMesh textInfo, textCostPrice;

	private string sCostType,sAotemanName, sAotemanIconName;
	private int iCostPrice;
	private int modelId, modelType;

	#region 重写父类方法
	public override void Init ()
	{
		Instance = this;
		base.Init();
	}

	public override void Show ()
	{
		base.Show ();

		PayUIConfigurator.PayUIConfig (PayType.AoteZhaohuan, null, null, null, null, null, imageCloseBt, goZhaohuanButton.GetComponent<BoxCollider> (), null);

		if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe) {
			goZhaohuanButton.transform.Find("UI_libao01_hg").gameObject.SetActive(false); //审核版不显示按钮特效，更换特效时要改名称
		}
	}

	public override void Hide ()
	{
		base.Hide ();
		UIManager.Instance.HideModule (UISceneModuleType.AotemanZhaohuan);
	}

	public override void Back ()
	{
		AudioManger.Instance.PlaySound (AudioManger.SoundName.CloseBtClick);
		Hide ();
	}
	#endregion

	#region  数据处理
	public void InitData(int modelId)
	{
		this.modelId = modelId;
		this.modelType = IDTool.GetModelType (modelId);
		sAotemanName = ModelData.Instance.GetName(modelId);
		sAotemanIconName = ModelData.Instance.GetPlayerIcon(modelId);
		roleSprite.SetSprite (ModelData.Instance.GetRoleIcon (modelId));
		sCostType = ModelData.Instance.GetZhaohuanCostType(modelId);
		iCostPrice = ModelData.Instance.GetZhaohuanCost(modelId);

		//textInfo.text = "召唤"+ sAotemanName + "\n痛打小怪兽";
		textInfo.text = "Call "+ sAotemanName;
		imageAoteIcon.SetSprite (sAotemanIconName);
		textCostPrice.text = iCostPrice.ToString();

		if(sCostType.CompareTo("Coin") == 0)
			imageCostType.SetSprite("coin_icon");
		if(sCostType.CompareTo("Jewel") == 0 || sCostType.CompareTo("AoteMuCard") == 0 || sCostType.CompareTo("AoteFuCard") == 0)
			imageCostType.SetSprite("jewel_icon");
	}

	void ZhaohuanAoteman()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		
		if(sCostType.CompareTo("Coin") == 0)
		{
			if(iCostPrice > PlayerData.Instance.GetItemNum(PlayerData.ItemType.Coin))
			{
				GiftPackageControllor.Instance.Show(PayType.CoinGift, ZhaohuanAoteman);
				CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_Coin,"State","自动弹出","Level",PlayerData.Instance.GetSelectedLevel().ToString());
				return;
			}
			else
			{
				PlayerData.Instance.ReduceItemNum(PlayerData.ItemType.Coin, iCostPrice);
			}
		}
		else if(sCostType.CompareTo("Jewel") == 0)
		{
			if(iCostPrice > PlayerData.Instance.GetItemNum(PlayerData.ItemType.Jewel))
			{
				GiftPackageControllor.Instance.Show(PayType.JewelGift, ZhaohuanAoteman);
				CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_Jewel,"State","自动弹出","Level",PlayerData.Instance.GetSelectedLevel().ToString());
				return;
			}
			else
			{
				PlayerData.Instance.ReduceItemNum(PlayerData.ItemType.Jewel, iCostPrice);
			}
		}

		Hide();
		PlayerData.Instance.UpdateModelState(modelId + 1);

		//自定义事件.
		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.Player_WakeUp, "Role call", ModelData.Instance.GetName (modelId));
	}
	#endregion

	#region 按钮控制
	void CloseButtonOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.CloseBtClick);
		Hide();
		return;
	}
	
	void ZhaohuanButtonOnClick()
	{
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);
		ZhaohuanAoteman();
	}
	#endregion
}
