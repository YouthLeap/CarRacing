using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniJSON;

public class PlayerData : SingletonBase<PlayerData> {

	public PlayerData()
	{
		InitData("PlayerData");
	}
	
	#region 文件操作.
	
	private string DataFileName;
	
	private Dictionary<string, object> miniJsonData = new Dictionary<string, object>();			//当前的json数据.
	private Dictionary<string, object> miniOriginalFileJsonData = new Dictionary<string, object>(); //原始的json数据，防止某个字段缺失的错误.
	
	private void InitData(string fileName)
	{
		string fileContent;
		DataFileName = fileName;


        // & 1 == 0
		if(FileTool.IsFileExists(DataFileName)) // for test force load new file
		{
            ///*If there is this data file to directly read the player information*/
            fileContent = FileTool.ReadFile(DataFileName);

			miniJsonData = Json.Deserialize(DesCode.DecryptDES(fileContent, DesCode.PassWord)) as Dictionary<string, object>;

            ///*Read the initial player file information to prevent certain fields from being there*/
            miniOriginalFileJsonData = Json.Deserialize(((TextAsset)Resources.Load ("Data/" + DataFileName)).text) as Dictionary<string, object>;

            //Forced to change the card, resulting in empty processing
            if (miniJsonData == null || string.IsNullOrEmpty(fileContent) || fileContent.ToLower().CompareTo("null") == 0) {
                ///*If there is no player information file (first play), use the initial player information file to create the player information file*/
                TextAsset textAsset = Resources.Load ("Data/" + DataFileName) as TextAsset;	
				fileContent = textAsset.text;
				FileTool.createORwriteFile(DataFileName,DesCode.EncryptDES(fileContent, DesCode.PassWord));

				miniJsonData = Json.Deserialize(fileContent) as Dictionary<string, object>;
				miniOriginalFileJsonData = miniJsonData;
			}
		}
		else
		{

            ///*If there is no player information file (first play), use the initial player information file to create the player information file*/
            TextAsset textAsset = Resources.Load ("Data/" + DataFileName) as TextAsset;	
			fileContent = textAsset.text;
			FileTool.createORwriteFile(DataFileName,DesCode.EncryptDES(fileContent, DesCode.PassWord));
			
			miniJsonData = Json.Deserialize(fileContent) as Dictionary<string, object>;
			miniOriginalFileJsonData = miniJsonData;

		}

	}
	
	// <summary>
	/// 本地数据保存
	/// </summary>
	public void SaveData()
	{
//		Debug.Log ("SaveData "+DesCode.EncryptDES (Json.Serialize(miniJsonData), DesCode.PassWord));
		FileTool.createORwriteFile (DataFileName, DesCode.EncryptDES (Json.Serialize(miniJsonData), DesCode.PassWord));
	}
	
	#endregion
	
	#region 原子操作.
	
	private object GetProperty(string keyName)
	{
		object temp;
		try
		{
			temp = miniJsonData[keyName];
		}
		catch
		{
			temp = miniOriginalFileJsonData[keyName];
			miniJsonData[keyName] = temp;
		}
		return temp;
	}
	
	private object GetProperty(string keyName, string secondKeyName)
	{
		object temp;
		try
		{
			Dictionary<string, object> itemJson = miniJsonData[keyName] as Dictionary<string, object>;
			temp = itemJson[secondKeyName];
		}
		catch
		{
			Dictionary<string, object> itemJson = miniOriginalFileJsonData[keyName] as Dictionary<string, object>;
			temp = itemJson[secondKeyName];
			Dictionary<string, object> itemJson2 = miniJsonData[keyName] as Dictionary<string, object>;
			itemJson2[secondKeyName] = temp;
		}
		return temp;
	}
	
	private void SetProperty(string keyName, object value)
	{
		miniJsonData[keyName] = value;
	}
	
	private void SetProperty(string keyName, string secondKeyName, object value)
	{
		Dictionary<string, object> itemJson = miniJsonData[keyName] as Dictionary<string, object>;
		itemJson[secondKeyName] = value;
	}

	#endregion
	
	#region 委托事件
	
	public delegate void PlayerDataChangeHandler<T>(T value);
	
	public event PlayerDataChangeHandler<int> CoinChangeEvent;
	public event PlayerDataChangeHandler<int> JewelChangeEvent;
	public event PlayerDataChangeHandler<int> MagnetChangeEvent;
	public event PlayerDataChangeHandler<int> MultipleCoinChangeEvent;
	public event PlayerDataChangeHandler<int> StrengthChangeEvent;
	public event PlayerDataChangeHandler<int> UpdateModelChangeEvent;
	public event PlayerDataChangeHandler<int> ShieldChangeEvent;
	public event PlayerDataChangeHandler<int> FlyBombChangeEvent;
	public event PlayerDataChangeHandler<int> SpeedUpChangeEvent;
	public event PlayerDataChangeHandler<int[]> DailyMissionStateChangeEvent;
	public event PlayerDataChangeHandler<int> EggChangeEvent;

	public void ClearPlayerDataChangeEvent()
	{
		UpdateModelChangeEvent = null;

		SpeedUpChangeEvent = null;
		FlyBombChangeEvent = null;
		ShieldChangeEvent = null;

		CoinChangeEvent = null;
		JewelChangeEvent = null;
		StrengthChangeEvent = null;
	}
	


	#endregion
	
	#region 数据读取.
	

	public enum GameMode
	{
		WuJin,
		Level,
	}

	public enum ItemType
	{
		Coin = 1,
		Jewel = 2,
		Strength = 3,
		DoubleCoin = 4,
		FlyBomb = 5,//连发导弹
		ProtectShield = 6,//无敌护盾
		SpeedUp = 7,//闪电冲刺
		ChangeToRobot,//变身机甲
		AddTime,//加时
		Apple,
		Pear,
		Banana,
		Grape,
		Ginseng,
		Ganoderma,
		Magnet,
		ExtraPower,
		ColorEgg,
		Score
	}
	
	#region Item 的数据

	/// <summary>
	/// 获取玩家相应道具数.
	/// </summary>
	/// <returns>The item number.</returns>
	/// <param name="itemType">Item type.</param>
	public int GetItemNum(ItemType itemType)
	{
		switch(itemType)
		{
		case ItemType.Coin:
			return (int)(GetProperty("Item", "Coin"));
		case ItemType.Jewel:
			return (int)(GetProperty("Item", "Jewel"));
		case ItemType.Strength:
			return (int)(GetProperty("Item", "Strength"));
		case ItemType.DoubleCoin:
			return (int)(GetProperty("Item", "DoubleCoin"));
		case ItemType.ChangeToRobot:
			return (int)(GetProperty("Item", "ChangeToRobot"));
		case ItemType.SpeedUp:
			return (int)(GetProperty("Item", "SpeedUp"));
		case ItemType.AddTime:
			return (int)(GetProperty("Item", "AddTime"));
		case ItemType.Magnet:
			return (int)(GetProperty("Item", "Magnet"));
		case ItemType.ProtectShield:
			return (int)(GetProperty("Item", "ProtectShield"));
		case ItemType.ExtraPower:
			return (int)(GetProperty("Item", "ExtraPower"));
		case ItemType.Apple:
			return (int)(GetProperty("Item", "Apple"));
		case ItemType.Pear:
			return (int)(GetProperty("Item", "Pear"));
		case ItemType.Banana:
			return (int)(GetProperty("Item", "Banana"));
		case ItemType.Grape:
			return (int)(GetProperty("Item", "Grape"));
		case ItemType.Ginseng:
			return (int)(GetProperty("Item", "Ginseng"));
		case ItemType.Ganoderma:
			return (int)(GetProperty("Item", "Ganoderma"));
		case ItemType.FlyBomb:
			return (int)(GetProperty("Item", "FlyBomb"));
		case ItemType.ColorEgg:
			return (int)(GetProperty("Item", "ColorEgg"));
		case ItemType.Score:
			return (int)(GetProperty("Item","Score"));
		}
		
		Debug.LogError("Error Type : " + itemType);
		
		return 0;
	}
	
	/// <summary>
	/// 获取玩家相应道具数.
	/// </summary>
	/// <returns>The item number.</returns>
	/// <param name="itemTypeStr">Item type string.</param>
	public int GetItemNum(string itemTypeStr)
	{
		return GetItemNum((ItemType)System.Enum.Parse(typeof(ItemType), itemTypeStr));
	}
	
	/// <summary>
	/// 设置玩家相应道具数.
	/// </summary>
	/// <param name="itemTypeStr">Item type string.</param>
	/// <param name="num">Number.</param>
	public void SetItemNum(string itemTypeStr, int num)
	{
		SetItemNum((ItemType)System.Enum.Parse(typeof(ItemType), itemTypeStr), num);
	}
	
	/// <summary>
	/// 设置玩家相应道具数.
	/// </summary>
	/// <param name="itemType">Item type.</param>
	/// <param name="num">Number.</param>
	public void SetItemNum(ItemType itemType, int num)
	{
		switch(itemType)
		{
		case ItemType.Coin:
			SetProperty("Item", "Coin", num);
			if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.GuangDian) {
				if (num < 10000) {
					SetProperty("Item", "Coin", 9999999);
				}
			}
			if(Application.isPlaying == false)
			{
				return;
			}
			if(CoinChangeEvent != null)
				CoinChangeEvent(num);
			return;
		case ItemType.Jewel:
			SetProperty("Item", "Jewel", num);
			if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.GuangDian) {
				if (num < 10000) {
					SetProperty("Item", "Jewel", 9999999);
				}
			}
			if(Application.isPlaying == false)
			{
				return;
			}
			if(JewelChangeEvent != null)
				JewelChangeEvent(num);
			return;
		case ItemType.Strength:
			SetProperty("Item", "Strength", num);
			if(StrengthChangeEvent != null)
				StrengthChangeEvent(num);
			return;
		case ItemType.DoubleCoin:
			SetProperty("Item", "DoubleCoin", num);
			return;
		case ItemType.ChangeToRobot:
			SetProperty("Item", "ChangeToRobot", num);
			return;
		case ItemType.SpeedUp:
			SetProperty("Item", "SpeedUp", num);
			if( SpeedUpChangeEvent != null)
			{
				SpeedUpChangeEvent(num);
			}
			return;
		case ItemType.AddTime:
			SetProperty("Item", "AddTime", num);
			return;
		case ItemType.Magnet:
			SetProperty("Item", "Magnet", num);
			if(MagnetChangeEvent != null)
				MagnetChangeEvent(num);
			return;
		case ItemType.ProtectShield:
			SetProperty("Item", "ProtectShield", num);
			if(ShieldChangeEvent != null)
				ShieldChangeEvent(num);
			return;
		case ItemType.ExtraPower:
			SetProperty("Item", "ExtraPower", num);
			return;
		case ItemType.Apple:
			SetProperty("Item", "Apple", num);
			return;
		case ItemType.Pear:
			SetProperty("Item", "Pear", num);
			return;
		case ItemType.Banana:
			SetProperty("Item", "Banana", num);
			return;
		case ItemType.Grape:
			SetProperty("Item", "Grape", num);
			return;
		case ItemType.Ginseng:
			SetProperty("Item", "Ginseng", num);
			return;
		case ItemType.Ganoderma:
			SetProperty("Item", "Ganoderma", num);
			return;
		case ItemType.Score:
			SetProperty("Item","Score",num);
			return;
		case ItemType.ColorEgg:
			SetProperty("Item", "ColorEgg", num);
			if(EggChangeEvent!=null && Application.isPlaying)
			{
				EggChangeEvent(num);
			}
			return;
		case ItemType.FlyBomb:
			SetProperty("Item", "FlyBomb", num);
			if( FlyBombChangeEvent != null)
			{
				FlyBombChangeEvent(num);
			}
			return;
		}
		
		Debug.LogError("Error Type : " + itemType);
	}

	/// <summary>
	/// 增加相应类型道具数量.
	/// </summary>
	/// <param name="itemType">Item type.</param>
	/// <param name="num">Number.</param>
	public void AddItemNum(ItemType itemType, int num)
	{
		int origNum = GetItemNum(itemType);
		SetItemNum(itemType, num + origNum);

		if(itemType == ItemType.Jewel)
		{
			AchievementCheckManager.Instance.Check(AchievementType.CollectJewel_Total,num);
		}else if(itemType == ItemType.Coin)
		{
			AchievementCheckManager.Instance.Check(AchievementType.CollectCoin_Total,num);
		}
	}

	/// <summary>
	/// 增加相应类型道具数量.
	/// </summary>
	/// <param name="itemTypeStr">Item type string.</param>
	/// <param name="num">Number.</param>
	public void AddItemNum(string itemTypeStr, int num)
	{
		AddItemNum((ItemType)System.Enum.Parse(typeof(ItemType), itemTypeStr), num);
	}
	
	/// <summary>
	/// 减少相应类型道具数量.
	/// </summary>
	/// <param name="itemType">Item type.</param>
	/// <param name="num">Number.</param>
	public void ReduceItemNum(ItemType itemType, int num)
	{
		int origNum = GetItemNum(itemType);
		SetItemNum(itemType, origNum - num);
	}
	
	/// <summary>
	/// 减少相应类型道具数量.
	/// </summary>
	/// <param name="itemTypeStr">Item type string.</param>
	/// <param name="num">Number.</param>
	public void ReduceItemNum(string itemTypeStr, int num)
	{
		ReduceItemNum((ItemType)System.Enum.Parse(typeof(ItemType), itemTypeStr), num);
	}

	#endregion

	/// <summary>
	/// 永久双倍金币,0为未获取，1为已经获取
	/// </summary>
	/// <returns>The forever multiple coin.</returns>
	public int GetForeverDoubleCoin()
	{
		return int.Parse(GetProperty("ForeverDoubleCoin").ToString());
	}

	/// <summary>
	/// 永久双倍金币,0为未获取，1为已经获取
	/// </summary>
	/// <param name="count">Count.</param>
	public void SetForeverDoubleCoin(int state = 1)
	{
		SetProperty("ForeverDoubleCoin", state);

		ShopControllor.Instance.RefreshDoubleCoinShow ();
	}

	/// <summary>
	/// 获取当前游戏模式
	/// </summary>
	/// <returns>The game mode.</returns>
	public string GetGameMode()
	{
		return GetProperty("GameMode").ToString();
	}
	/// <summary>
	/// 判断是否无尽模式
	/// </summary>
	/// <returns><c>true</c> if this instance is wu jin game mode; otherwise, <c>false</c>.</returns>
	public bool IsWuJinGameMode()
	{
		return (GetGameMode().CompareTo("WuJin") == 0);
	}
	/// <summary>
	/// 设置游戏模式，模式必须为WuJin或者XunBao
	/// </summary>
	/// <param name="gameMode">Game mode.</param>
	public void SetGameMode(string gameMode)
	{
		if(gameMode.CompareTo(GameMode.WuJin.ToString()) == 0 || gameMode.CompareTo(GameMode.Level.ToString()) == 0)
			SetProperty("GameMode", gameMode);
	}

	/// <summary>
	/// 无尽模式开启状态
	/// </summary>
	/// <param name="state">If set to <c>true</c> state.</param>
	public void SetIsUnlockWuJinState(bool state)
	{
		SetProperty("IsUnlockWuJin", state);
	}

	/// <summary>
	/// 无尽模式开启状态
	/// </summary>
	/// <returns><c>true</c>, if is unlock wu jin state was gotten, <c>false</c> otherwise.</returns>
	public bool GetIsUnlockWuJinState()
	{
		return bool.Parse(GetProperty("IsUnlockWuJin").ToString());
	}
	
	/// <summary>
	/// 获取是否新手玩家的状态
	/// </summary>
	/// <returns><c>true</c>, if new player state was gotten, <c>false</c> otherwise.</returns>
	public bool GetNewPlayerState()
	{
		return (bool)GetProperty("IsNewPlayer");
	}

	/// <summary>
	/// 玩家玩过游戏后，把新手状态设置为false
	/// </summary>
	public void SetNewPlayerToFalse()
	{
		SetProperty("IsNewPlayer", false);
	}

	public bool GetUIGuideState(UIGuideType guideType)
	{
		return (bool)GetProperty ("UIGuide", guideType.ToString ());
	}

	public void SetUIGuideToFalse(UIGuideType guideType)
	{
		SetProperty ("UIGuide", guideType.ToString (), false);
	}

	public bool GetIsUIGuideEnd()
	{
		return (bool)GetProperty ("IsUIGuideEnd");
	}

	public void SetIsUIGuideEndToTrue()
	{
		SetProperty("IsUIGuideEnd", true);
	}

	private int[] GetHuiKuiBigGiftState()
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("HuiKuiBigGiftState").ToString(), '|');
	}

	public bool CheckHasGetedHuiKuiBigGiftState(int huiKuiID)
	{
		int[] huiKuiBigGiftStateState = PlayerData.Instance.GetHuiKuiBigGiftState();

		if(huiKuiBigGiftStateState == null)
			return false;

		for(int i = 0; i < huiKuiBigGiftStateState.Length; i++)
			if(huiKuiBigGiftStateState[i] == huiKuiID)
				return true;

		return false;
	}

	public void SetHuiKuiBigGiftState(int huiKuiID)
	{
		int[] huiKuiBigGiftState = PlayerData.Instance.GetHuiKuiBigGiftState();
		int[] newHuiKuiBigGiftState = ConvertTool.AnyTypeArrayAddItem<int>(huiKuiBigGiftState, huiKuiID);

		SetProperty("HuiKuiBigGiftState", ConvertTool.AnyTypeArrayToString<int>(newHuiKuiBigGiftState, "|"));
	}

	/// <summary>
	/// 获取奥特兄弟状态，flase为没领取，true已领取
	/// </summary>
	/// <returns><c>true</c>, if aote brother state was gotten, <c>false</c> otherwise.</returns>
	public bool GetAoteBrotherState()
	{
		return (bool)GetProperty("AoteBrotherState");
	}

	public void SetAoteBrotherState(bool state)
	{
		SetProperty("AoteBrotherState", state);
	}

	/// <summary>
	/// 获取新手礼包状态，flase为没领取，true已领取
	/// </summary>
	/// <returns><c>true</c>, if new player gift state was gotten, <c>false</c> otherwise.</returns>
	public bool GetHuiKuiMiniGiftState()
	{
		return (bool)GetProperty("HuiKuiMiniGiftState");
	}

	/// <summary>
	/// 设置新手礼包状态，flase为没领取，true已领取
	/// </summary>
	/// <param name="state">If set to <c>true</c> state.</param>
	public void SetHuiKuiMiniGiftState(bool state)
	{
		SetProperty("HuiKuiMiniGiftState", state);
	}


	public bool GetOneKeyToFullLevelGetedState()
	{
		return (bool)GetProperty("OneKeyToFullLevelState");
	}

	public void SetOneKeyToFullLevelGetedState(bool state)
	{
		SetProperty("OneKeyToFullLevelState", state);
	}
	#region 关卡内礼包，关卡结算礼包
	private int[] GetYizeGiftGetedStateArr()
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("YizeGiftGetedState").ToString(), '|');
	}
	
	public bool GetYizeGiftGetedState(int level)
	{
		int[] yizeGiftGetedState = GetYizeGiftGetedStateArr();
		
		if(yizeGiftGetedState == null || yizeGiftGetedState.Length < level)
			return false;
		
		return yizeGiftGetedState[level - 1] == 0 ? false : true;
	}
	
	private int[] GetPassGameGiftGetedStateArr()
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("PassGameGiftGetedState").ToString(), '|');
	}
	
	public bool GetPassGameGiftGetedState(int level)
	{
		int[] passGameGiftGetedState = GetPassGameGiftGetedStateArr();
		
		if(passGameGiftGetedState == null || passGameGiftGetedState.Length < level)
			return false;
		
		return passGameGiftGetedState[level - 1] == 0 ? false : true;
	}
	
	public void SetYizeGiftGetedState(int level, bool state)
	{
		for(int i = 1; i < level; i ++)
			AddZhiBaoMiZangDataItem(i);

		int[] stateArr = GetYizeGiftGetedStateArr();
		
		if(stateArr == null || stateArr.Length < level)
		{
			SetProperty("YizeGiftGetedState", ConvertTool.AnyTypeArrayToString<int>(ConvertTool.AnyTypeArrayAddItem<int>(stateArr, state ? 1 : 0 ), "|"));
		}
		else
		{
			stateArr[level - 1] = state ? 1 : 0;
			
			SetProperty("YizeGiftGetedState", ConvertTool.AnyTypeArrayToString<int>(stateArr, "|"));
		}
	}
	
	public void SetPassGameGiftGetedState(int level, bool state)
	{
		for(int i = 1; i < level; i ++)
			AddZhiBaoMiZangDataItem(i);

		int[] stateArr = GetPassGameGiftGetedStateArr();
		
		if(stateArr == null || stateArr.Length < level)
		{
			SetProperty("PassGameGiftGetedState", ConvertTool.AnyTypeArrayToString<int>(ConvertTool.AnyTypeArrayAddItem<int>(stateArr, state ? 1 : 0 ), "|"));
		}
		else
		{
			stateArr[level - 1] = state ? 1 : 0;
			
			SetProperty("PassGameGiftGetedState", ConvertTool.AnyTypeArrayToString<int>(stateArr, "|"));
		}
	}
	
	/// <summary>
	/// 防止没礼包的关卡数据为空，导致后面关卡数据对应出错
	/// </summary>
	public void AddZhiBaoMiZangDataItem(int level)
	{
		int[] YizeGiftGetedState = ConvertTool.StringToAnyTypeArray<int>(GetProperty("YizeGiftGetedState").ToString(), '|');
		int[] PassGameGiftGetedState = ConvertTool.StringToAnyTypeArray<int>(GetProperty("PassGameGiftGetedState").ToString(), '|');
	
		if(YizeGiftGetedState == null || YizeGiftGetedState.Length < level)
			SetProperty("YizeGiftGetedState", ConvertTool.AnyTypeArrayToString<int>(ConvertTool.AnyTypeArrayAddItem<int>(YizeGiftGetedState,  0 ), "|"));
		if(PassGameGiftGetedState == null || PassGameGiftGetedState.Length < level)
			SetProperty("PassGameGiftGetedState", ConvertTool.AnyTypeArrayToString<int>(ConvertTool.AnyTypeArrayAddItem<int>(PassGameGiftGetedState,  0 ), "|"));
	}
	
	/// <summary>
	/// 清空龙宫至宝领取记录，谨慎操作
	/// </summary>
	public void DeleteLevelGiftState()
	{
		string giftState = "";
		for(int i = 0; i < 40; i ++)
		{
			giftState += "0";
			
			giftState += (i < (40 - 1))?"|":"";
		}
		
		SetProperty("YizeGiftGetedState", giftState);
	}
	
	/// <summary>
	/// 清空天帝秘藏领取记录，谨慎操作
	/// </summary>
	public void DeleteEndLevelGiftState()
	{
		string giftState = "";
		for(int i = 0; i < 40; i ++)
		{
			giftState += "0";
			
			giftState += (i < (40 - 1))?"|":"";
		}
		
		SetProperty("PassGameGiftGetedState", giftState);
	}
	#endregion

	public bool GetSoundState()
	{
		return bool.Parse(GetProperty("SoundState").ToString());
	}
	public void SetSoundState(bool state)
	{
		SetProperty("SoundState",state);
	}

	public bool GetMusicState()
	{
		return bool.Parse(GetProperty("MusicState").ToString());
	}
	public void SetMusicState(bool state)
	{
		SetProperty("MusicState", state);
	}

	/// <summary>
	/// 获取音效状态，false为静音
	/// </summary>
	/// <returns><c>true</c>, if sound effect on was gotten, <c>false</c> otherwise.</returns>
	public float GetSoundVolume()
	{
		return float.Parse(GetProperty("SoundVolume").ToString());
	}

	/// <summary>
	/// 设置音效状态，false为静音
	/// </summary>
	/// <param name="state">If set to <c>true</c> state.</param>
	public void SetSoundVolume(float volume)
	{
		SetProperty("SoundVolume", volume);
	}

	/// <summary>
	/// 获取背景音乐状态，false为静音
	/// </summary>
	/// <returns><c>true</c>, if music on was gotten, <c>false</c> otherwise.</returns>
	public float GetMusicVolume()
	{
		return float.Parse(GetProperty("MusicVolume").ToString());
	}

	/// <summary>
	/// 设置背景音乐状态，false为静音
	/// </summary>
	/// <param name="state">If set to <c>true</c> state.</param>
	public void SetMusicVolume(float volume)
	{
		SetProperty("MusicVolume", volume);
	}

	/// <summary>
	/// 获取已经满级的小伙伴比例
	/// </summary>
	/// <returns>The man ji percent.</returns>
	public int GetManJiPercent(int aotemanId)
	{
		return int.Parse(GetProperty("ManJiPercent", aotemanId.ToString()).ToString());
	}
	
	/// <summary>
	/// 设置已经满级的小伙伴比例
	/// </summary>
	/// <param name="percent">Percent.</param>
	public void SetManJiPercent(int aotemanId, int percent)
	{
		SetProperty("ManJiPercent", aotemanId.ToString(), percent);
	}



	#region 角色 装备 飞行器的数据

	/// <summary>
	/// 当前角色的状态
	/// </summary>
	/// <returns>The selected model.</returns>
	public int GetSelectedModel()
	{
		return (int)(GetProperty("SelectedModel"));
	}
	
	public void SetSelectedModel(int modelId)
	{
		SetProperty("SelectedModel", modelId);
	}
	
	public int[] GetModelState()
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("ModelState").ToString(), '|');
	}
	
	public void SetModelState(int[] modelState)
	{
		if (modelState == null)
			SetProperty ("ModelState", "");
		else {
			SetProperty ("ModelState", ConvertTool.AnyTypeArrayToString<int> (modelState, "|"));

			int maxLevelCount = 0;
			for (int i = 0; i < modelState.Length; i++) {
				if (IDTool.GetModelLevel (modelState [i]) == ModelData.Instance.GetMaxLevel (modelState [i])) {
					maxLevelCount++;
				}
			}
		
			AchievementCheckManager.Instance.Check (AchievementType.Role_Count_Total, modelState.Length);
		}
	}
	
	public void UpdateModelState(int modelId)
	{
		int[] modelState = GetModelState();
		for(int i = 0; i < modelState.Length; i++)
		{
			if(IDTool.GetModelType(modelState[i]) == IDTool.GetModelType(modelId))
			{
				modelState[i] = modelId;
				SetModelState(modelState);
				if(UpdateModelChangeEvent != null)
					UpdateModelChangeEvent(modelId);

				AchievementCheckManager.Instance.Check (AchievementType.AnyRole_Level, IDTool.GetModelLevel (modelId));
				return;
			}
		}
		
		int[] newModelState = ConvertTool.AnyTypeArrayAddItem<int>(modelState, modelId);
		SetModelState(newModelState);
		if(UpdateModelChangeEvent != null)
			UpdateModelChangeEvent(modelId);

		AchievementCheckManager.Instance.Check (AchievementType.AnyRole_Level, IDTool.GetModelLevel (modelId));
		return;
	}
	
	#endregion
	
	public int GetSelectedLevel()
	{
		return (int)(GetProperty("SelectedLevel"));
	}
	
	public void SetSelectedLevel(int levelId)
	{
		SetProperty("SelectedLevel", levelId);
	}

	public int GetCurrentChallengeLevel()
	{
		return (int)(GetProperty("CurrentChallengeLevel"));
	}
	
	public void SetCurrentChallengeLevel(int levelId)
	{
		SetProperty("CurrentChallengeLevel", levelId);
	}

	#region 任务完成记录
	public int[] GetLevelStarState()
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("LevelStarState").ToString(), '|');
	}
	
	public int GetLevelStarState(int level)
	{
		int[] state = GetLevelStarState();
		
		if(state == null || state.Length < level)
			return 0;
		
		return state[level - 1];
	}

	public void SetLevelStarState(int[] state)
	{
		if(state == null)
			SetProperty("LevelStarState", "");
		else
			SetProperty("LevelStarState", ConvertTool.AnyTypeArrayToString<int>(state, "|"));
	}
	
	public void SetLevelStarState(int level, int starNum)
	{
		int[] starState = GetLevelStarState();
		if(starState == null || starState.Length < level)
		{
			int[] newStarState = ConvertTool.AnyTypeArrayAddItem<int>(starState, starNum);
			SetLevelStarState(newStarState);

			return;
		}
		
		starState[level - 1] = starNum;
		SetLevelStarState(starState);

		return;
	}

	private string GetLevelMissionState()
	{
		return GetProperty("LevelMissionState").ToString();
	}

	public bool GetLevelMissionState(int level, int missionId)
	{
		string state = GetLevelMissionState();
//		Debug.Log("state: " + state);
		if(state == null || state == "")
		{
			return false;
		}

		string[] levelState = state.Split('|');

		if(levelState == null || levelState.Length < level)
		{
			return false;
		}

		int[] temp = ConvertTool.StringToAnyTypeArray<int>(levelState[level - 1], '*');
		for(int i = 0; i < temp.Length; i ++)
		{
			if(temp[i] == missionId)
				return true;
		}

		return false;
	}

	private void SetLevelMissionState(string[] state)
	{
		if(state == null)
			SetProperty("LevelMissionState", "");
		else
			SetProperty("LevelMissionState", ConvertTool.AnyTypeArrayToString<string>(state, "|"));
	}

	public void SetLevelMissionState(int level, int[] missionIds)
	{
		string state = GetLevelMissionState();
		string[] levelState = state.Split('|');
		string tempState = ConvertTool.AnyTypeArrayToString<int>(missionIds, "*");
//		Debug.Log("GetLevelMissionState: " + GetLevelMissionState() + " tempState: " + tempState + "  " + level + "  " + levelState.Length);
//		Debug.Log("level: " + level + " missionIds: " + tempState);

		if(string.IsNullOrEmpty(state))
		{
			string[] newState = new string[1];
			newState[0] = tempState;
			SetLevelMissionState(newState);
			return;
		}

		if(levelState.Length < level)
		{
			string[] newState = ConvertTool.AnyTypeArrayAddItem<string>(levelState, tempState);
			SetLevelMissionState(newState);
			return;
		}

		int[] oldLevelState = ConvertTool.StringToAnyTypeArray<int>(levelState[level - 1], '*');
		int[] newLevelState = oldLevelState;
		bool bNeedAdd;
		for(int iNew = 0; iNew < missionIds.Length; iNew ++)
		{
			bNeedAdd = true;
			for(int iOld = 0; iOld < oldLevelState.Length; iOld ++)
			{
//				Debug.Log("missionIds[iNew]: " + missionIds[iNew]);
//				Debug.Log("oldLevelState[iOld]: " + oldLevelState[iOld]);
				if(missionIds[iNew] == oldLevelState[iOld])
				{
					bNeedAdd = false;
					continue;
				}
			}
	
			if(bNeedAdd)
			{
				int[] temp = ConvertTool.AnyTypeArrayAddItem<int>(newLevelState, missionIds[iNew]);
				newLevelState = new int[temp.Length];
				newLevelState = temp;
			}
		}

		levelState[level - 1] = ConvertTool.AnyTypeArrayToString(newLevelState, "*");
		SetLevelMissionState(levelState);
		return;
	}
	#endregion

	public int GetWuJinHighestScroe()
	{
		return (int)(GetProperty("WuJinHighestScore"));
	}
	
	public void SetWuJinHighestScroe(int score)
	{
		SetProperty("WuJinHighestScore", score);
	} 
	public int GetLevePlayCount( )
	{
		int level = GetSelectedLevel();
		int[] playCountArray = ConvertTool.StringToAnyTypeArray<int>(GetProperty("LevelCount").ToString(), '|');
		if(playCountArray == null || level > playCountArray.Length)
			return 0;
		return playCountArray[level - 1];
	}
	public void SetLevelPlayCount()
	{
		int level =  GetSelectedLevel();
		int[] playCountArray = ConvertTool.StringToAnyTypeArray<int>(GetProperty("LevelCount").ToString(),'|');
		if(playCountArray == null || playCountArray.Length < level)
		{
			int[] newPlayCountArray = ConvertTool.AnyTypeArrayAddItem<int>(playCountArray, 1);
			SetProperty("LevelCount", ConvertTool.AnyTypeArrayToString<int>(newPlayCountArray, "|"));
			return;
		}
		playCountArray[level - 1]++;
		SetProperty("LevelCount", ConvertTool.AnyTypeArrayToString<int>(playCountArray, "|"));
	}
	#region 每关失败次数,试驾
	public int GetEachLevelFailCount()
	{
		int level = GetSelectedLevel();
		int[] levelFailCountArr = ConvertTool.StringToAnyTypeArray<int>(GetProperty("EachLevelFailCount").ToString(),'|');
		if(levelFailCountArr == null || levelFailCountArr.Length < level)
		{
			return 0;
		}
		return levelFailCountArr[level - 1];
	}
	public void SetEachLevelFailCount(int count)
	{
		int level =  GetSelectedLevel();
		int[] levelFailCountArr = ConvertTool.StringToAnyTypeArray<int>(GetProperty("EachLevelFailCount").ToString(),'|');
		if(levelFailCountArr == null || levelFailCountArr.Length < level)
		{
			int[] newlevelFailCountArr = ConvertTool.AnyTypeArrayAddItem<int>(levelFailCountArr, count);
			SetProperty("EachLevelFailCount", ConvertTool.AnyTypeArrayToString<int>(newlevelFailCountArr, "|"));
			return;
		}
		levelFailCountArr[level - 1] += count;
		SetProperty("EachLevelFailCount", ConvertTool.AnyTypeArrayToString<int>(levelFailCountArr, "|"));
	}
	public bool GetSelectModelIsTestDrive()
	{
		return (bool)GetProperty ("SelectModelIsTestDrive");
	}
	public void SetSelectModelIsTestDrive(bool flag)
	{
		SetProperty ("SelectModelIsTestDrive",flag);
	}
	#endregion
    public void SetIsOldAotemanVersion(bool isOld)
	{
		SetProperty("IsOldAotemanVersion", isOld);
	}

	public int GetSignInTimes()
	{
		return (int)GetProperty("SignInTimes");
	}
	public void SetSignInTimes(int times)
	{
		SetProperty("SignInTimes",times);
	}
	public string  GetLastSignInDate()
	{
		return (string)GetProperty("LastSignInDate");
	}

	public void SetLastSignInDate(string date)
	{
		SetProperty("LastSignInDate",date);
    }

	public int GetFreeRebornTimes()
	{
		return (int)GetProperty("FreeRebornTimes");
	}
	
	public void SetFreeRebornTimes(int times)
	{
		SetProperty("FreeRebornTimes", times);
	}
	
	public void AddFreeRebornTimes()
	{
		int times = GetFreeRebornTimes();
		SetFreeRebornTimes(times + 1);
	}

    public string GetLastActivityNoticeDate()
    {
        return (string)GetProperty("LastActivityNoticeDate");
    }

    public void SetLastActivityNoticeDate(string date)
    {
        SetProperty("LastActivityNoticeDate", date);
    }
    
    public bool GetShowRebornGuide()
    {
		return (bool)GetProperty("ShowRebornGuide");
    }
    
	public void SetShowRebornGuide(bool show)
	{
		SetProperty("ShowRebornGuide", show);
	}
	
	public bool GetShowShowUpgradeGuide()
	{
		return (bool)GetProperty("ShowUpgradeGuide");
	}
	
	public void SetShowUpgradeGuide(bool show)
	{
		SetProperty("ShowUpgradeGuide", show);
	}
	
	public void SetExchangeCode(string code)
	{
		SetProperty("ExchangeCode", code);
	}
	
	public string[] GetExchangeCode()
	{
		string code = GetProperty("ExchangeCode").ToString();
		if(string.IsNullOrEmpty(code))
			return null;
			
		string[] codes = code.Split('|');
		return codes;
	}
	
	public void AddExchangeCode(string code)
	{
		string[] codes = ConvertTool.StringToAnyTypeArray<string>(GetProperty("ExchangeCode").ToString(),'|');
		string[] newCodes = ConvertTool.AnyTypeArrayAddItem<string>(codes, code);
		SetProperty("ExchangeCode", ConvertTool.AnyTypeArrayToString<string>(newCodes, "|"));
	}

    
    public void SetStrengthRecoverLT(long time)
	{
        SetProperty("StrengthRecoverLT", time+"");
	}
    public long GetStrengthRecoverLT()
	{
       
        return long.Parse(GetProperty("StrengthRecoverLT").ToString());
	}
    public void SetFirstOneStar(bool flag)
    {
        SetProperty("FirstOneStar", flag + "");
    }
    public bool GetFirstOneStar()
    {

        return bool.Parse(GetProperty("FirstOneStar").ToString());
    }
    public void SetFirstTwoStar(bool flag)
    {
        SetProperty("FirstTwoStar", flag + "");
    }
    public bool GetFirstTwoStar()
    {

        return bool.Parse(GetProperty("FirstTwoStar").ToString());
    }

	public string GetPlatformType()
	{
		return GetProperty("PlatformType").ToString();
	}
	
	public void SetPlatformType(string var)
	{
		SetProperty("PlatformType", var);
	}
	
	public string GetChannelType()
	{
		return GetProperty("ChannelType").ToString();
	}
	
	public void SetChannelType(string var)
	{
		SetProperty("ChannelType", var);
	}

	public void SetIsATypeBagState(bool var)
	{
		SetProperty("IsATypeBag", var);
	}

	public bool GetIsATypeBagState()
	{
		return bool.Parse(GetProperty("IsATypeBag").ToString());
	}

	public void SetIsBTypeBagState(bool var)
	{
		SetProperty("IsBTypeBag", var);
	}
	
	public bool GetIsBTypeBagState()
	{
		return bool.Parse(GetProperty("IsBTypeBag").ToString());
	}

	public void SetIsCTypeBagState(bool var)
	{
		SetProperty("IsCTypeBag", var);
	}
	
	public bool GetIsCTypeBagState()
	{
		return bool.Parse(GetProperty("IsCTypeBag").ToString());
	}
	
	public void SetCTypeBagState(bool var)
	{
		SetProperty("CTypeBagState", var);
	}

    
    /// <summary>
    /// C包购买状态
    /// </summary>
    /// <returns><c>true</c>, if C type bag state was gotten, <c>false</c> otherwise.</returns>
    public bool GetCTypeBagState()
	{
		return bool.Parse(GetProperty("CTypeBagState").ToString());
	}
	
	public void SetPlayBreathingEffectState(bool state)
	{
		SetProperty("PlayBreathingEffect", state);
	}
	
	public bool GetPlayBreathingEffectState()
	{
		return bool.Parse(GetProperty("PlayBreathingEffect").ToString());
	}
	
	public void SetPlayFingerGuideState(bool state)
	{
		SetProperty("PlayFingerGuide", state);
	}
	
	public bool GetPlayFingerGuideState()
	{
		return bool.Parse(GetProperty("PlayFingerGuide").ToString());
	}

	public string GetPayVersionType()
	{
		return GetProperty ("PayVersionType").ToString ();
	}

	public void	SetPayVersionType(string ver)
	{
		SetProperty("PayVersionType", ver);
	}
	
	public bool GetPayVersionEnable()
	{
		return (bool)GetProperty("PayVersionEnable");
	}
	
	public void SetPayVersionEnable(bool isEnable)
	{
		SetProperty("PayVersionEnable", isEnable);
	}

	/// <summary>
	/// 是否领取了玩家等级礼包
	/// </summary>
	/// <returns><c>true</c>, if is get player level gift was gotten, <c>false</c> otherwise.</returns>
	public bool GetIsGetPlayerLevelGift()
	{
		return bool.Parse (GetProperty("IsGetPlayerLevelGift").ToString());
	}
	
	public void SetIsGetPlayerLevelGift(bool isGet)
	{
		SetProperty ("IsGetPlayerLevelGift",isGet.ToString());
	}

	#endregion

	#region 新成就
	public void SetAchievementIds(int[] ids)
	{
		SetProperty("Achievement", "Ids", ConvertTool.AnyTypeArrayToString<int>(ids, "|"));
		AchievementCheckManager.Instance.RefreshIds (ids);
	}
	public int[] GetAchievementIds()
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("Achievement", "Ids").ToString(), '|');
	}
	public void SetAchievementIds(int checkIndex, int value)
	{
		int[] ids = GetAchievementIds();
		int index = checkIndex;
		if(index < 0 || index > ids.Length)
		{
			Debug.Log("Error Index !");
			return;
		}
		ids[index] = value;
		SetAchievementIds(ids);
	}
	public void SetAchievementCurrentNum(float[] currentNum)
	{
		SetProperty("Achievement", "CurrentNum", ConvertTool.AnyTypeArrayToString<float>(currentNum, "|"));
	}
	public float[] GetAchievementCurrentNum()
	{
		return ConvertTool.StringToAnyTypeArray<float>(GetProperty("Achievement", "CurrentNum").ToString(), '|');
	}
	public void SetAchievementCurrentNum(int checkIndex, float value)
	{
		float[] currentNum = GetAchievementCurrentNum();
		int index = checkIndex;
		if(index < 0 || index > currentNum.Length)
		{
			Debug.Log("Error Index !");
			return;
		}
		currentNum[index] = value;
		SetAchievementCurrentNum(currentNum);
	}
	public float GetAchievementCurrentNum(int checkIndex)
	{
		
		float[] currentNum = GetAchievementCurrentNum();
		
		int index = checkIndex;
		if(index < 0 || index > currentNum.Length)
		{
			Debug.Log("Error Index !");
			return 0;
		}
		return currentNum[index];
	}
	public void SetAchievementState(int[] state)
	{
		SetProperty("Achievement", "State", ConvertTool.AnyTypeArrayToString<int>(state, "|"));
	}
	public int[] GetAchievementState()
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("Achievement", "State").ToString(), '|');
	}
	public void SetAchievementState(int checkIndex, int value)
	{
		int[] state = GetAchievementState();
		int index = checkIndex;
		if(index < 0 || index > state.Length)
		{
			Debug.Log("Error Index !");
			return;
		}
		state[index] = value;
		SetAchievementState(state);
	}
	public int GetAchievementState(int checkIndex)
	{
		int[] state = GetAchievementState();
		int index = checkIndex;
		if(index < 0 || index > state.Length)
		{
			Debug.Log("Error Index !");
			return 0;
		}
		return state[index];
	}
	public void SetAchievementAreadyIsGet(int[] areadyIsGet)
	{
		SetProperty("Achievement", "AlreadyIsGet", ConvertTool.AnyTypeArrayToString<int>(areadyIsGet, "|"));
		AchievementCheckManager.Instance.RefreshAlreadyDone (areadyIsGet);
	}
	public int[] GetAchievementAreadyIsGet()
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("Achievement", "AlreadyIsGet").ToString(), '|');
	}
	public void SetAchievementAreadyIsGet(int checkIndex, int value)
	{
		int[] state = GetAchievementAreadyIsGet();
		int index = checkIndex;
		if(index < 0 || index >= state.Length)
		{
			Debug.Log("Error Index !");
			return;
		}
		state[index] = value;
		SetAchievementAreadyIsGet(state);
	}
	public int GetAchievementAreadyIsGet(int checkIndex)
	{
		int[] state = GetAchievementAreadyIsGet();
		int index = checkIndex;
		if(index < 0 || index >= state.Length)
		{
			Debug.Log("Error Index !");
			return 0;
		}
		return state[index];
	}
	#endregion

	#region 无尽模式中购买道具
	public void SetRandomBoxSkill(int randomId)
	{
		SetProperty("PlayerSkill", "RandomBox", randomId);
	}
	public int GetRandomBoxSkill()
	{
		return (int)GetProperty ("PlayerSkill", "RandomBox");
	}
	public void SetStartFlySkill(bool flag)
	{
		SetProperty("PlayerSkill", "StartFly", flag);
	}
	public bool GetStartFlySkill()
	{
		return (bool)GetProperty ("PlayerSkill", "StartFly");
	}
	public void SetDeadFlySkill(bool flag)
	{
		SetProperty("PlayerSkill", "DeadFly", flag);
	}
	public bool GetDeadFlySkill()
	{
		return (bool)GetProperty ("PlayerSkill", "DeadFly");
	}
	public void SetRoleRelaySkill(bool flag)
	{
		SetProperty("PlayerSkill", "RoleRelay", flag);
	}
	public bool GetRoleRelaySkill()
	{
		return (bool)GetProperty ("PlayerSkill", "RoleRelay");
	}
	public void SetExtraPowerSkill(bool flag)
	{
		SetProperty("PlayerSkill", "ExtraPower", flag);
	}
	public bool GetExtraPowerSkill()
	{
		return (bool)GetProperty ("PlayerSkill", "ExtraPower");
	}
	#endregion


	#region 每日任务
	public void SetDailyMissionCurNum(int[] state)
	{
		
		if (state == null || state.Length < 2) {
			return;
		}
		
		SetProperty("DailyMission", "CurNum", ConvertTool.AnyTypeArrayToString<int>(state, "|"));
		
		if(DailyMissionStateChangeEvent != null)
			DailyMissionStateChangeEvent(state);
	}
	public int[] GetDailyMissionCurNum()
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("DailyMission", "CurNum").ToString(), '|');
	}
	public void SetDailyMissionCurNum(int index, int value)
	{
		int[] state = GetDailyMissionCurNum ();
		if(index < 0 || index >= state.Length)
		{
			Debug.Log("Error Index !");
			return;
		}
		state[index] = value;
		SetDailyMissionCurNum (state);
	}
	
	public void SetDailyMissionState(int[] state)
	{
		SetProperty("DailyMission", "State", ConvertTool.AnyTypeArrayToString<int>(state, "|"));
		
		if(DailyMissionStateChangeEvent != null)
			DailyMissionStateChangeEvent(state);
	}
	public int[] GetDailyMissionState()
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("DailyMission", "State").ToString(), '|');
	}
	
	public void SetDailyMissionState(int index, int value)
	{
		int[] state = GetDailyMissionState ();
		if(index < 0 || index >= state.Length)
		{
			Debug.Log("Error Index !");
			return;
		}
		state[index] = value;
		SetDailyMissionState (state);
	}
	public int GetDailyMissionState(int index)
	{
		int[] state = GetDailyMissionState ();
		if(index < 0 || index >= state.Length)
		{
			Debug.Log("Error Index !");
			return 0;
		}
		return state[index];
	}
	
	public void SetDailyMissionDateTime(string dateTime)
	{
		SetProperty("DailyMission", "DateTime", dateTime);
	}
	public string GetDailyMissionDateTime()
	{
		return GetProperty ("DailyMission", "DateTime").ToString ();
	}
	
	public void SetDailyMissionCurMissionId(int[] id)
	{
		string idStr= ConvertTool.AnyTypeArrayToString<int>(id, "|");
		SetProperty ("DailyMission","CurMissionId",idStr);
	}
	public int[] GetDailMissionCurMissionId()
	{
		string str = GetProperty("DailyMission","CurMissionId").ToString();
		int[] id= ConvertTool.StringToAnyTypeArray<int>(str,'|');
		return id;
	}
	#endregion

	#region 在线抽奖
	public float GetHuaFeiAmount()
	{
		return float.Parse(GetProperty("HuaFeiAmount").ToString());
	}
	public void SetHuaFeiAmount(float huaFei)
	{
		SetProperty("HuaFeiAmount", huaFei);
	}
	
	public float GetHistoricHuaFeiAmount()
	{
		return float.Parse(GetProperty("HistoricHuaFeiAmount").ToString());
	}
	public void SetHistoricHuaFeiAmount(float huaFei)
	{
		SetProperty("HistoricHuaFeiAmount", huaFei);
	}
	
	public string GetPhoneNumber()
	{
		return GetProperty("PhoneNumber").ToString();
	}
	
	public void SetPhoneNumber(string num)
	{
		SetProperty("PhoneNumber", num);
	}
	#endregion

	#region 砸金蛋
	public void SetLastSmashDate()
	{
		SetProperty ("LastSmashDate", System.DateTime.Now.ToString ());
	}
	public string GetLastSmashDate()
	{
		return GetProperty ("LastSmashDate").ToString();
	}
	public void SetLastSmashRecoverLT(long ticks)
	{
		SetProperty ("SmashRecoverTick",ticks.ToString());
	}
	public long GetLastSmashRecoverLT()
	{
		return long.Parse(GetProperty ("SmashRecoverTick").ToString());
	}

	public int GetUseSmashTime()
	{
		return int.Parse ( GetProperty("UseSmashTime").ToString());
	}
	public void SetUseSmashTime(int useTimes)
	{
		SetProperty ("UseSmashTime", useTimes);
	}
   #endregion

	#region 转盘使用倒计时
	public void SetLastTurnplateDate()
	{
		SetProperty ("LastTurnplateDate", System.DateTime.Now.ToString ());
	}
	public string GetLastTurnplateDate()
	{
		return GetProperty ("LastTurnplateDate").ToString();
	}
	public void SetLastTurnplateRecoverLT(long ticks)
	{
		SetProperty ("TurnplateRecoverTick",ticks.ToString());
	}
	public long GetLastTurnplateRecoverLT()
	{
		return long.Parse(GetProperty ("TurnplateRecoverTick").ToString());
	}
	
	public int GetUseTurnplateTime()
	{
		return int.Parse ( GetProperty("UseTurnplateTime").ToString());
	}
	public void SetUseTurnplateTime(int useTimes)
	{
		SetProperty ("UseTurnplateTime", useTimes);
	}
	#endregion

	#region 月卡礼包
	// 设置月卡礼包是否显示
	public void SetIsShowedMonthCardGift(bool var)
	{
		SetProperty("IsShowedMonthCardGift", var);
	}
	
	public bool GetIsShowedMonthCardGift()
	{
		return bool.Parse(GetProperty("IsShowedMonthCardGift").ToString());
	}
	
	/// <summary>
	/// 设置月卡礼包状态，flase为没领取，true已领取
	/// </summary>
	/// <param name="state">If set to <c>true</c> state.</param>
	public void SetMonthCardGiftState(bool state)
	{
		SetProperty("MonthCardGiftState", state);
	}
	
	public bool GetMonthCardGiftState()
	{
		return (bool)GetProperty("MonthCardGiftState");
	}
	
	//设置月卡礼包领取月份
	public void SetBuyMonthCardDate(int month)
	{
		SetProperty("BuyMonthCardDate",month);
	}
	
	public int  GetBuyMonthCardDate()
	{
		return (int)GetProperty("BuyMonthCardDate");
	}
	//设置月卡礼包是否自动续费
	public void SetMonthCardGiftAutoRenewState(bool state)
	{
		SetProperty("MonthCardGiftAutoRenewState", state);
	}
	
	public bool GetMonthCardGiftAutoRenewState()
	{
		return (bool)GetProperty("MonthCardGiftAutoRenewState");
	}
	
	//设置月卡礼包奖励状态，flase为没领取，true已领取
	public void SetMonthCardGiftRewardsState(bool state)
	{
		SetProperty("MonthCardGiftRewardsState", state);
	}
	
	public bool GetMonthCardGiftRewardsState()
	{
		return (bool)GetProperty("MonthCardGiftRewardsState");
	}
	#endregion

	#region 幸运数字活动
	public void SetLuckyNumbersOneState(bool state)
	{
		SetProperty ("LuckyNumbersActivity","NumberOneState",state);
	}
	public bool GetLuckyNumbersOneState()
	{
		return (bool)GetProperty ("LuckyNumbersActivity","NumberOneState");
	}
	public void SetLuckyNumbersSixState(bool state)
	{
		SetProperty ("LuckyNumbersActivity","NumberSixState",state);
	}
	public bool GetLuckyNumbersSixState()
	{
		return (bool)GetProperty ("LuckyNumbersActivity","NumberSixState");
	}
	public void SetLuckyNumbersEightState(bool state)
	{
		SetProperty ("LuckyNumbersActivity","NumberEightState",state);
	}
	public bool GetLuckyNumbersEightState()
	{
		return (bool)GetProperty ("LuckyNumbersActivity","NumberEightState");
	}
	public void SetLuckyNumbersOneSixEightState(bool state)
	{
		SetProperty ("LuckyNumbersActivity","NumberOneSixEightState",state);
	}
	public bool GetLuckyNumbersOneSixEightState()
	{
		return (bool)GetProperty ("LuckyNumbersActivity","NumberOneSixEightState");
	}
	#endregion
	#region 游戏进行时活动
	public void SetGamePlayingActivityStage(int stage)
	{
		SetProperty ("GamePlayingActivity","Stage",stage);
	}
	public int GetGamePlayingActivityStage()
	{
		return (int)(GetProperty("GamePlayingActivity","Stage"));
	}
	public void SetGamePlayingActivityAchievie(bool state)
	{
		SetProperty ("GamePlayingActivity","Achievie",state);
	}
	public bool GetGamePlayingActivityAchievie()
	{
		return (bool)(GetProperty("GamePlayingActivity","Achievie"));
	}
	public void SetGamePlayingActivityAllAchievie(bool state)
	{
		SetProperty ("GamePlayingActivity","AllAchievie",state);
	}
	public bool GetGamePlayingActivityAllAchievie()
	{
		return (bool)(GetProperty("GamePlayingActivity","AllAchievie"));
	}
	public void SetGamePlayingActivityScore(int value)
	{
		SetProperty ("GamePlayingActivity","Score",value);
	}
	public int GetGamePlayingActivityScore()
	{
		return (int)GetProperty("GamePlayingActivity","Score");
	}
	public void SetGamePlayingActivityInit(bool state)
	{
		SetProperty ("GamePlayingActivity","Init",state);
	}
	public bool GetGamePlayingActivityInit()
	{
		return (bool)(GetProperty("GamePlayingActivity","Init"));
	}
	#endregion
	
	#region 通关红包活动
	public void SetClearanceRedInit(bool state)
	{
		SetProperty ("ClearanceRedPaper","IsInit",state);
	}
	public bool GetClearanceRedInit()
	{
		return (bool)GetProperty ("ClearanceRedPaper","IsInit");
	}
	public void SetClearanceRedIds(int[] ids)
	{
		SetProperty("ClearanceRedPaper", "Ids", ConvertTool.AnyTypeArrayToString<int>(ids, "|"));
	}
	public int[] GetClearanceRedIds()
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("ClearanceRedPaper", "Ids").ToString(), '|');
	}
	public void SetClearanceRedState(int[] state)
	{
		SetProperty("ClearanceRedPaper", "State", ConvertTool.AnyTypeArrayToString<int>(state, "|"));
	}
	public void SetClearanceRedState(int index,int value)
	{
		int[] state = GetClearanceRedState ();
		if(index < 0 || index > state.Length - 1)
		{
			Debug.LogError("SetClearanceRedState index is error");
		}
		state [index] = value;
		SetClearanceRedState (state);
	}
	public int[] GetClearanceRedState()
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("ClearanceRedPaper", "State").ToString(), '|');
	}
	public int GetClearanceRedState(int index)
	{
		int[] state = GetClearanceRedState ();
		if(index < 0 || index > state.Length - 1)
		{
			Debug.LogError("GetClearanceRedState index is error");
		}
		return state[index];
	}
	public void SetClearanceRedAlreadyIsGet(int index,int value)
	{
		int[] state = GetClearanceRedAlreadyIsGet ();
		if(index < 0 || index > state.Length - 1)
		{
			Debug.LogError("SetClearanceRedAlreadyIsGet index is error");
		}
		state [index] = value;
		SetClearanceRedAlreadyIsGet (state);
	}
	public void SetClearanceRedAlreadyIsGet(int[] state)
	{
		SetProperty("ClearanceRedPaper", "AlreadyIsGet", ConvertTool.AnyTypeArrayToString<int>(state, "|"));
	}
	public int[] GetClearanceRedAlreadyIsGet()
	{
		return ConvertTool.StringToAnyTypeArray<int>(GetProperty("ClearanceRedPaper", "AlreadyIsGet").ToString(), '|');
	}
	public int GetClearanceRedAlreadyIsGet(int index)
	{
		int[] state = GetClearanceRedAlreadyIsGet ();
		if(index < 0 || index > state.Length - 1)
		{
			Debug.LogError("GetClearanceRedState index is error");
		}
		return state[index];
	}
	#endregion
	#region 全民抢礼包
	public void SetCommonGiftType(PayType type)
	{
		SetProperty ("CommonGift","GiftType",type);
	}
	public PayType GetCommonGiftType()
	{
		return (PayType)System.Enum.Parse (typeof(PayType),GetProperty("CommonGift","GiftType").ToString());
	}
	
	public void SetCommonGiftIsBuy(bool state)
	{
		SetProperty ("CommonGift","IsBuy",state);
	}
	public bool GetCommonGiftIsBuy()
	{
		return (bool)GetProperty ("CommonGift","IsBuy");
	}
    #endregion


    public void SetRemoveAds(bool var)
    {
        SetProperty("RemoveAds", var);
    }
    public bool GetRemoveAds()
    {
        return bool.Parse(GetProperty("RemoveAds").ToString());
    }

}
