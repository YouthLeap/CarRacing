using UnityEngine;
using System.Collections;
public enum SceneType
{
	UIScene,
	GameScene
};
public class GlobalConst {

	public static bool FirstIn = true;

	public static bool IsSignIn = false;

	public static UISceneModuleType ShowModuleType = UISceneModuleType.MainInterface;

	public static SceneType SceneName = SceneType.UIScene;
	public static float PlayTime = 0.4f;
	public static int UnlockWujinLevel
	{
		get{
			if(PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe
				|| PlatformSetting.Instance.PayVersionType == PayVersionItemType.GuangDian
				|| PlatformSetting.Instance.PayVersionType == PayVersionItemType.ChangWan )
				return 1;
			else
				return 8;
		}
	}

	//UICanvas使用的坐标位置
	public static Vector3 ShowPos = Vector3.zero;
	public static Vector3 LeftHidePos = new Vector3 (-1000, 0, 0);
	public static Vector3 RightHidePos = new Vector3 (1000, 0, 0);
	public static Vector3 TopHidePos = new Vector3 (0, 1000, 0);
	public static Vector3 BottomHidePos = new Vector3 (0, -1000, 0);

	/// <summary>
	/// 游戏准备标志
	/// </summary>
	public static bool IsReady = false;
	
	/// <summary>
	/// 游戏UI准备标志
	/// </summary>
	public static bool IsUIReady = false;

	//场景赛道
	static public float[] lanes = {-0.74f, -0.37f, 0.0f, 0.37f, 0.74f};

	public static bool CharaterDetailGuideEnabled{
		get{
			if (GlobalConst.IsSignIn && PlayerData.Instance.GetCurrentChallengeLevel () == 3 && PlayerData.Instance.GetUIGuideState (UIGuideType.CharaterDetailGuide))
				return true;
			
			return false;
		}
	}
	
	public static bool LeftRoleGuideEnabled{
		get{
			if (PlayerData.Instance.GetUIGuideState (UIGuideType.LeftRoleGuide) && PlayerData.Instance.GetCurrentChallengeLevel () == 4) 
				return true;
			
			return false;
		}
	}
	
	public static bool StartGameGuideEnabled{
		get{
			if (PlayerData.Instance.GetNewPlayerState () && PlayerData.Instance.GetCurrentChallengeLevel () == 1) 
				return true;
			
			return false;
		}
	}

	public static int MaxLevel
	{
		get {
			return LevelData.Instance.GetDataRow ();
		}
	}
}