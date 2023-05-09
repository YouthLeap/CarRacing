using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GameController : MonoBehaviour {

	public static GameController Instance;
	
	void Awake()
	{
		Instance = this;
	}
	
	void Start()
	{
		GameData.Instance.Init();
		//PlayerController.Instance.Init();
		//DropItemManage.Instance.Init();
		GameUIManager.Instance.Init ();

		//游戏准备标志
		GlobalConst.IsReady = true;
		LiBaoProp.isShow=false;
	
	}

    /// <summary>
    ///  After the interface is loaded.
    /// </summary>
    public void StartFromLoading () {
		//PlayerController.Instance.Born();	

		if (PlayerData.Instance.IsWuJinGameMode ()) {
            AndroidPackage.instance.LogLevel(AndroidPackage.LogLevelType.start_level, 0);
            CollectInfoEvent.StartLevel(0);
		} else {
            AndroidPackage.instance.LogLevel(AndroidPackage.LogLevelType.start_level, PlayerData.Instance.GetSelectedLevel());
            CollectInfoEvent.StartLevel(PlayerData.Instance.GetSelectedLevel());
		}

		//GameUIManager.Instance.ShowModule(UISceneModuleType.GameSkill);
		CarCameraFollow.Instance.ShowStartAnim();
		EventLayerController.Instance.SetEventLayer (EventLayer.Nothing);
	}

	/// <summary>
	/// 正式开始跑动
	/// </summary>
	public void StartGo()
	{
		GameData.Instance.IsPause = false;
	}

	public void PauseGame()
	{
		GameData.Instance.IsPause = true;
		AudioManger.Instance.PauseMusic ();
		AudioManger.Instance.PauseSound();
	}
	
	public void ResumeGame()
	{
		GameData.Instance.IsPause = false;
		AudioManger.Instance.UnPauseMusic ();
		AudioManger.Instance.UnPauseSound();
	}
		
	/// <summary>
	/// 死掉的函数。。。.
	/// </summary>
	public void Dead()
	{
		if(GameData.Instance.IsPause)
			return;
		GameData.Instance.IsPause = true;
		DOTween.PauseAll();
		EventLayerController.Instance.SetEventLayer (EventLayer.Nothing);
		StartCoroutine ("DelayShowGameReborn");
		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.Dead, "选择模式", PlayerData.Instance.GetGameMode (), "选择关卡", PlayerData.Instance.GetSelectedLevel ().ToString ());

		AudioManger.Instance.PlaySound (AudioManger.SoundName.AotemanDeath);
	}

	IEnumerator DelayShowGameReborn()
	{
		yield return new WaitForSeconds (0.7f);
		GameUIManager.Instance.ShowModule(UISceneModuleType.GameReborn);
	}

	public void Reborn()
	{
		DOTween.PlayAll ();
	    GameData.Instance.IsPause = false;

		CarManager.Instance.Reborn();
	}
	
	public void FinishGame(bool isWin= true)
	{

        AndroidPackage.instance.ShowAdmob();

        int analyticsLevel = 0;
        if (PlayerData.Instance.IsWuJinGameMode() == false)
        {
            analyticsLevel = PlayerData.Instance.GetSelectedLevel();
        }

        if (isWin)
        {
            AndroidPackage.instance.LogLevel(AndroidPackage.LogLevelType.win_level, analyticsLevel);
        }        
        else
        {
            AndroidPackage.instance.LogLevel(AndroidPackage.LogLevelType.lose_level, analyticsLevel);
        }

		//MissionBoard.Instance.CheckMission ();

		PublicSceneObject.Instance.IsReceiveAndroidBackKeyEvent = false;

		CarManager.Instance.SetFinalRank();
		GameData.Instance.IsWin = isWin;
		CreatePropManager.Instance.gameObject.SetActive(false);

		if(isWin)
		{
			MissionBoard.Instance.CheckMission();
			PlayerCarControl.Instance.carMove.animManager.Win ();
			HintInGameControllor.Instance.InitData(HintInGameControllor.HintType.Win, AfterWinEffect);
		}else
		{
			PlayerCarControl.Instance.carMove.animManager.Fail ();
			HintInGameControllor.Instance.InitData(HintInGameControllor.HintType.Fail, AfterWinEffect);
		}

		GameUIManager.Instance.ShowModule(UISceneModuleType.HintInGame);
	}

	public void RestartGame()
	{
		if(PlayerData.Instance.GetGameMode().CompareTo(PlayerData.GameMode.Level.ToString()) == 0 && AutoGiftChecker.ForeseeAutoGiftCheck(AutomaticGiftName.DoubleCoin) && AutoGiftChecker.iEnterLevelSelectTimes == 0)
		{
			GlobalConst.ShowModuleType = UISceneModuleType.LevelSelect;
			GlobalConst.SceneName = SceneType.UIScene;
			LoadingPage.Instance.LoadScene ();
		}else
		{
			GlobalConst.SceneName = SceneType.GameScene;
			LoadingPage.Instance.LoadScene ();
		}
	}

	void AfterWinEffect()
	{
		if(PlayerData.Instance.IsWuJinGameMode())
		{
			ShowEndGame();
		}else
		{
			GameUIManager.Instance.ShowModule(UISceneModuleType.GameRank);
		}
	}

	public void ShowEndGame()
	{
		AdmobAdsSet.Instance.showInterstitial ();
		GameData.Instance.IsPause = true;
		GameUIManager.Instance.ShowModule (UISceneModuleType.GameEndingScore);
		Invoke("Hide3DObject",0.8f);
	}

	private void Hide3DObject()
	{
		RoadPathManager.Instance.DestoryResource();
		GameObject.Find("Car").SetActive(false);
		GameObject.Find("Manager").SetActive(false);
	}
	
}
