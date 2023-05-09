using UnityEngine;
using System.Collections;

public class CommonGiftAward : UIBoxBase {
	
	public static CommonGiftAward Instance = null;

	public tk2dSprite[] goIconTypeArr;
	public EasyFontTextMesh[] goGiftCount;
	
	private PlayerData.ItemType[] AwardTypeArr;
	private int[] AwardCountArr;
	private string[] AwardIconNameArr;
	
	public override void Init ()
	{
		base.Init ();
		Instance = this;
		int id = 1;
		AwardTypeArr = CommonGiftAwardData.Instance.GetAwardType (id);
		AwardCountArr = CommonGiftAwardData.Instance.GetAwardCount (id);
		AwardIconNameArr = CommonGiftAwardData.Instance.GetAwardIconName (id);

		for(int i = 0; i < AwardTypeArr.Length; i ++)
		{
			if (AwardIconNameArr [i] == "coin_icon") {
				goIconTypeArr [i].SetSprite ("coin_2");
			} else {
				goIconTypeArr [i].SetSprite (AwardIconNameArr [i]);
			}
			goGiftCount [i].text = AwardCountArr [i].ToString() ;
		}
		base.Init();
	}
	
	public override void Show ()
	{
		base.Show ();
	}
	
	public override void Hide ()
	{
		gameObject.SetActive (false);
		UIManager.Instance.HideModule (UISceneModuleType.CommonGiftAward);
		//弹完C包后，显示关卡信息
		if (GlobalConst.StartGameGuideEnabled) {
			UIGuideControllor.Instance.Show (UIGuideType.LevelInfoGuide);
		}
	}
	
	
	public void GetOnClick()
	{
		for(int i = 0; i < AwardTypeArr.Length; i ++)
		{
			PlayerData.Instance.AddItemNum (AwardTypeArr [i], AwardCountArr [i]);
		}
		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.CommonGift, "State", "领取奖励次数", "Level", PlayerData.Instance.GetSelectedLevel ().ToString ());
		Hide ();
		AudioManger.Instance.PlaySound(AudioManger.SoundName.ButtonClick);		
	}
}
