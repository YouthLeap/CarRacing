using UnityEngine;
using System.Collections;
using UnityEditor;

public class PlayerDataEditor : EditorWindow {
	
	private int CoinCount = PlayerData.Instance.GetItemNum(PlayerData.ItemType.Coin);
	private int JewelCount = PlayerData.Instance.GetItemNum(PlayerData.ItemType.Jewel);
	private int ScoreCount= PlayerData.Instance.GetItemNum(PlayerData.ItemType.Score);
	private int ProtectShieldCount = PlayerData.Instance.GetItemNum(PlayerData.ItemType.ProtectShield);
	private int FlyBombCount = PlayerData.Instance.GetItemNum(PlayerData.ItemType.FlyBomb);
	private int SpeedUpCount = PlayerData.Instance.GetItemNum(PlayerData.ItemType.SpeedUp);
	private int AppleCount = PlayerData.Instance.GetItemNum (PlayerData.ItemType.Apple);
	private int BananaCount = PlayerData.Instance.GetItemNum (PlayerData.ItemType.Banana);
	private int PearCount = PlayerData.Instance.GetItemNum (PlayerData.ItemType.Pear);
	private int CurCallengeLevel = PlayerData.Instance.GetCurrentChallengeLevel();
	private int CurStrength = PlayerData.Instance.GetItemNum(PlayerData.ItemType.Strength);
	private int SignInTimes = PlayerData.Instance.GetSignInTimes();
	private int ColorEgg = PlayerData.Instance.GetItemNum (PlayerData.ItemType.ColorEgg);
	private string ModelStateStr = ConvertTool.AnyTypeArrayToString<int>(PlayerData.Instance.GetModelState(), "|");
	private bool IsATypeBag = PlayerData.Instance.GetIsATypeBagState();
	private bool IsBTypeBag = PlayerData.Instance.GetIsBTypeBagState();
	private bool IsCTypeBag = PlayerData.Instance.GetIsCTypeBagState();
	private bool PlayBreathingEffect = PlayerData.Instance.GetPlayBreathingEffectState();
	private bool PlayFingerGuide = PlayerData.Instance.GetPlayFingerGuideState();
	private bool IsMonthCardGift = PlayerData.Instance.GetIsShowedMonthCardGift();
	private bool IsMonthCardGiftAutoRenew = PlayerData.Instance.GetMonthCardGiftAutoRenewState();

	public bool isWujinModel = PlayerData.Instance.IsWuJinGameMode();

	private string[] Yunyinshang = {"电信", "联通", "移动"};
	private string[] PayVersion  = {"礼包白", "礼包黑", "审核版", "畅玩版", "广电版"};

	private string VersionFileName = PlayerData.Instance.GetPayVersionType();

	private int SelectedYunyinShang = 0;
	private int SelectedPayVersion = 0;

	[MenuItem ("DataSetting/用户数据设置", false)]
	static void OpenPlayerDataSetting () {
		PlayerDataEditor window = (PlayerDataEditor)EditorWindow.GetWindow (typeof (PlayerDataEditor));
	}

	[MenuItem ("DataSetting/用户数据设置", true)]
	static bool ValidateOpenPlayerDataSetting()
	{
		return FileTool.IsFileExists("PlayerData");
	}

	void OnFocus()
	{
		if (FileTool.IsFileExists ("PlayerData") == false)
			PlayerData.Instance.SaveData ();
		
		GetOriData();
	}

	void OnGUI () {

		GUILayout.Label ("用户数据设置", EditorStyles.boldLabel);

		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		CoinCount = EditorGUILayout.IntField ("金币数量", CoinCount, GUILayout.Height(20));
		JewelCount = EditorGUILayout.IntField ("钻石数量", JewelCount, GUILayout.Height(20));
		ScoreCount = EditorGUILayout.IntField ("积分数量", ScoreCount, GUILayout.Height(20));
		ProtectShieldCount = EditorGUILayout.IntField ("无敌护盾", ProtectShieldCount, GUILayout.Height(20));
		SpeedUpCount = EditorGUILayout.IntField ("闪电冲刺", SpeedUpCount, GUILayout.Height(20));
		FlyBombCount = EditorGUILayout.IntField ("连发导弹", FlyBombCount, GUILayout.Height(20));
		AppleCount = EditorGUILayout.IntField ("苹果", AppleCount, GUILayout.Height(20));
		BananaCount = EditorGUILayout.IntField ("香蕉", BananaCount, GUILayout.Height(20));
		PearCount = EditorGUILayout.IntField ("雪梨", PearCount, GUILayout.Height(20));
		ColorEgg = EditorGUILayout.IntField ("ColorEgg",ColorEgg,GUILayout.Height(20));
		CurStrength = EditorGUILayout.IntField ("体力数量", CurStrength, GUILayout.Height(20));
		SignInTimes = EditorGUILayout.IntField ("签到天数", SignInTimes, GUILayout.Height(20));
		ModelStateStr = EditorGUILayout.TextField ("角色等级", ModelStateStr, GUILayout.Height(20));
		CurCallengeLevel = EditorGUILayout.IntField ("开启关卡", CurCallengeLevel);

		IsATypeBag = EditorGUILayout.Toggle("A包", IsATypeBag, GUILayout.Height(20));
		IsBTypeBag = EditorGUILayout.Toggle("B包", IsBTypeBag, GUILayout.Height(20));
		IsCTypeBag = EditorGUILayout.Toggle("C包", IsCTypeBag, GUILayout.Height(20));
		IsMonthCardGift = EditorGUILayout.Toggle("月卡礼包", IsMonthCardGift, GUILayout.Height(20));
		IsMonthCardGiftAutoRenew = EditorGUILayout.Toggle("月卡礼包自动续费", IsMonthCardGiftAutoRenew, GUILayout.Height(20));

		PlayBreathingEffect = EditorGUILayout.Toggle("按钮呼吸效果", PlayBreathingEffect, GUILayout.Height(20));
		PlayFingerGuide = EditorGUILayout.Toggle("手指指导点击", PlayFingerGuide, GUILayout.Height(20));


		isWujinModel = EditorGUILayout.Toggle ("无尽模式",isWujinModel, GUILayout.Height(20));

		if(GUILayout.Button("应用用户数据"))
		{
			PlayerData.Instance.SetItemNum(PlayerData.ItemType.Coin, CoinCount);
			PlayerData.Instance.SetItemNum(PlayerData.ItemType.Jewel, JewelCount);
			PlayerData.Instance.SetItemNum(PlayerData.ItemType.Score, ScoreCount);
			PlayerData.Instance.SetItemNum(PlayerData.ItemType.ProtectShield, ProtectShieldCount);
			PlayerData.Instance.SetItemNum(PlayerData.ItemType.FlyBomb, FlyBombCount);
			PlayerData.Instance.SetItemNum(PlayerData.ItemType.SpeedUp, SpeedUpCount);
			PlayerData.Instance.SetItemNum(PlayerData.ItemType.Apple, AppleCount);
			PlayerData.Instance.SetItemNum(PlayerData.ItemType.Banana, BananaCount);
			PlayerData.Instance.SetItemNum(PlayerData.ItemType.Pear, PearCount);
			PlayerData.Instance.SetItemNum(PlayerData.ItemType.Strength, CurStrength);
			PlayerData.Instance.SetSignInTimes(SignInTimes);
			PlayerData.Instance.SetCurrentChallengeLevel(CurCallengeLevel);
			PlayerData.Instance.SetIsATypeBagState(IsATypeBag);
			PlayerData.Instance.SetIsBTypeBagState(IsBTypeBag);
			PlayerData.Instance.SetIsCTypeBagState(IsCTypeBag);
			PlayerData.Instance.SetPlayBreathingEffectState(PlayBreathingEffect);
			PlayerData.Instance.SetPlayFingerGuideState(PlayFingerGuide);
			PlayerData.Instance.SetItemNum(PlayerData.ItemType.ColorEgg,ColorEgg);
			PlayerData.Instance.SetIsShowedMonthCardGift(IsMonthCardGift);
			PlayerData.Instance.SetMonthCardGiftAutoRenewState(IsMonthCardGiftAutoRenew);

			if(isWujinModel)
			{
				PlayerData.Instance.SetGameMode(PlayerData.GameMode.WuJin.ToString());
			}else{
				PlayerData.Instance.SetGameMode(PlayerData.GameMode.Level.ToString());
			}

			//直接跳过的的关上加上0星级记录
			int[] mission = {0};
			for(int i = 1; i < CurCallengeLevel; i ++)
			{
				if(PlayerData.Instance.GetLevelStarState(i) == 0)
					PlayerData.Instance.SetLevelStarState(i, 0);
				if(!PlayerData.Instance.GetLevelMissionState(i, 1))
					PlayerData.Instance.SetLevelMissionState(i, mission);
			}
			//重置设置之前玩过的关卡为0星级记录
			for(int i = CurCallengeLevel; i <= 30; i ++)
				PlayerData.Instance.SetLevelStarState(i, 0);

			PayJsonData.Instance.SaveData();
			PlayerData.Instance.SaveData();
		}

		GUILayout.Space(25);

		GetOriData();
		GUILayout.Label ("运营商设置", EditorStyles.boldLabel, GUILayout.Width(100));
		SelectedYunyinShang = GUILayout.Toolbar(SelectedYunyinShang, Yunyinshang);
		
		switch(SelectedYunyinShang)
		{
		case 0:
			PlayerData.Instance.SetPlatformType("DianXin");
			break;
		case 1:
			PlayerData.Instance.SetPlatformType("LianTong");
			break;
		case 2:
			PlayerData.Instance.SetPlatformType("YiDong");
			break;
		default:
			break;
		}
		
		GUILayout.Space(25);
		
		GUILayout.Label ("版本设置", EditorStyles.boldLabel, GUILayout.Width(100));
		SelectedPayVersion = GUILayout.Toolbar(SelectedPayVersion, PayVersion);
		
		switch(SelectedPayVersion)
		{
		case 0:
			PlayerData.Instance.SetPayVersionType("LiBaoBai");
			break;
		case 1:
			PlayerData.Instance.SetPayVersionType("LiBaoHei");
			break;
		case 2:
			PlayerData.Instance.SetPayVersionType("ShenHe");
			break;
		case 3:
			PlayerData.Instance.SetPayVersionType("ChangWan");
			break;
		case 4:
			PlayerData.Instance.SetPayVersionType ("GuangDian");
			break;
		default:
			break;
		}

		PlayerData.Instance.SaveData();
	}

	void GetOriData()
	{
		string yunyinType = PlayerData.Instance.GetPlatformType();
		
		switch(yunyinType)
		{
		case "DianXin":
			SelectedYunyinShang = 0;
			break;
		case "LianTong":
			SelectedYunyinShang = 1;
			break;
		case "YiDong":
			SelectedYunyinShang = 2;
			break;
		default:
			SelectedYunyinShang = 0;
			break;
		}
		
		string verType = PlayerData.Instance.GetPayVersionType();
		
		switch(verType)
		{
		case "LiBaoBai":
			SelectedPayVersion = 0;
			break;
		case "LiBaoHei":
			SelectedPayVersion = 1;
			break;
		case "ShenHe":
			SelectedPayVersion = 2;
			break;
		case "ChangWan":
			SelectedPayVersion = 3;
			break;
		case "GuangDian":
			SelectedPayVersion = 4;
			break;
		}
	}
}
