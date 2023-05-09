using UnityEngine;
using System.Collections;
using PayBuild;

public class LoadingPage : MonoBehaviour {

	public static LoadingPage Instance;

	public ProgressBarNoMask ProgressBar;
	public EasyFontTextMesh ProgressText;
	//public Transform roleSprite;

	public EasyFontTextMesh TipText;

	private float curProgress;
	private AsyncOperation Async;

	public void InitScene ()
	{
		Instance = this;
		DontDestroyOnLoad (gameObject);

		PublicSceneObject.Instance.Init ();
		PublicSceneObject.Instance.IsReceiveAndroidBackKeyEvent = false;

		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.Loading, "state", "start  level" + PlayerData.Instance.GetSelectedLevel()+" To:"+GlobalConst.SceneName);

		StartCoroutine ("IEInitScene");
	}

	private void SetProgress()
	{
		if (curProgress < 50)
			curProgress += 5;
		else
			++ curProgress;
		ProgressText.text = curProgress + "%";
		float progressValue = curProgress / 100;
		ProgressBar.SetProgress (progressValue);

		//roleSprite.localPosition = new Vector3 (-280 + progressValue * 560, -121);
	}

    #region Scene jump
    IEnumerator IEInitScene()
	{
//		TipText.text = TipsData.Instance.GetContent (1);

		AudioManger.Instance.StopAll();

		gameObject.SetActive (true);
		curProgress = 0;
		while(curProgress < 100)
		{
			SetProgress ();
			yield return new WaitForEndOfFrame ();
		}

		yield return new WaitForSeconds (0.2f);
		LoadSceneComplete ();
	}

	public void LoadScene()
	{
        //		if (PlayerData.Instance.GetNewPlayerState ())
        //			TipText.text = TipsData.Instance.GetContent (1);
        //		else
        //			TipText.text = TipsData.Instance.GetRandomContent ();

        //Clear the commission
        PlayerData.Instance.ClearPlayerDataChangeEvent ();
		PublicSceneObject.Instance.ClearAndroidBackKeyEvent ();
        //Save game data
        PlayerData.Instance.SaveData ();
		ExchangeActivityData.Instance.SaveData();

        //Game ready logo
        GlobalConst.IsReady = false;
		GlobalConst.IsUIReady = false;

		gameObject.SetActive (true);
		StartCoroutine ("IELoadScene");
		PublicSceneObject.Instance.IsReceiveAndroidBackKeyEvent = false;

		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.Loading, "state", "start level" + PlayerData.Instance.GetSelectedLevel ()+" To:"+GlobalConst.SceneName);
	}

	IEnumerator IELoadScene()
	{
		AudioManger.Instance.StopAll();
		
		curProgress = 0;
		Async = Application.LoadLevelAsync (GlobalConst.SceneName.ToString ());
		while (curProgress < 100) {
			SetProgress ();
			yield return new WaitForEndOfFrame ();
		}
		
		yield return new WaitForSeconds (0.2f);
		LoadSceneComplete ();
	}

	/// <summary>
	/// 场景加载完成后调用此函数
	/// </summary>
	void LoadSceneComplete()
	{
		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.Loading, "state", "end level" + PlayerData.Instance.GetSelectedLevel ()+" To:"+GlobalConst.SceneName);
		ExchangeActivityData.Instance.InitFromServer ();

		StopCoroutine ("IEWaiting");
		StartCoroutine ("IEWaiting");
	}

	/// <summary>
	/// 加载完成UI场景后调用此函数.
	/// </summary>
	void LoadUISceneComplete()
	{
		UIManager.Instance.ShowModule (UISceneModuleType.PropertyDisplay);
		UIManager.Instance.ShowModule (UISceneModuleType.MainInterface);

		switch (GlobalConst.ShowModuleType) {
		case UISceneModuleType.MainInterface:
			//不过第一关, 不弹签到
			if (!GlobalConst.IsSignIn && PlayerData.Instance.GetCurrentChallengeLevel() > 1) {
				UIManager.Instance.ShowModule (UISceneModuleType.SignIn);
			}
			break;
		case UISceneModuleType.LockTip:
			LockTipControllor.Instance.InitData(LockTipType.UnlockTip);
			UIManager.Instance.ShowModule (UISceneModuleType.LockTip);
			break;
		case UISceneModuleType.LevelSelect:
			LevelInfoControllor.Instance.SetModelIndex (IDTool.GetModelType (PlayerData.Instance.GetSelectedModel ()) - 1);
			UIManager.Instance.ShowModule (UISceneModuleType.LevelSelect);
			break;
		case UISceneModuleType.LevelInfo:
			LevelInfoControllor.Instance.SetModelIndex (IDTool.GetModelType (PlayerData.Instance.GetSelectedModel ()) - 1);
			UIManager.Instance.ShowModule (UISceneModuleType.LevelSelect);
			LevelInfoControllor.Instance.InitData (PlayerData.Instance.GetCurrentChallengeLevel());
			UIManager.Instance.ShowModule (UISceneModuleType.LevelInfo);
			break;
		}
		AudioManger.Instance.PlayMusic(AudioManger.MusicName.UIBackground);
	}

	IEnumerator IEWaiting()
	{
		while (GlobalConst.IsReady == false || GlobalConst.IsUIReady == false) {
			yield return new WaitForSeconds (0.2f);
		}
		PublicSceneObject.Instance.IsReceiveAndroidBackKeyEvent = true;
		gameObject.SetActive (false);

		switch (GlobalConst.SceneName) {
		case SceneType.UIScene:
			LoadUISceneComplete ();
			break;
		case SceneType.GameScene:
			LoadGameSceneComplete ();
			break;
		}
	}
	
	/// <summary>
	/// 加载完成游戏场景后调用此函数.
	/// </summary>
	void LoadGameSceneComplete()
	{
		GameUIManager.Instance.ShowModule (UISceneModuleType.GamePlayerUI);
		GameController.Instance.StartFromLoading ();
		AudioManger.Instance.PlayMusic (AudioManger.MusicName.GameBackground);
	}
	#endregion
}