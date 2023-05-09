using UnityEngine;
using System.Collections;

public enum GameGuideStep{Left,Right,UseCurProp,UseFlyBmob,UseSpeedup}
public class GuideItem : ItemBase {
	public GameGuideStep curStep = GameGuideStep.Left;
	public override void GetItem (PropControl  propCon= null)
	{
		if(propCon==null)
		{
			return;
		}
		if(propCon.isPlayer == false)
		{
			return;
		}

		if(PlayerData.Instance.IsWuJinGameMode())
		{
			return;
		}

		switch(curStep)
		{
		case GameGuideStep.Left:
			if(PlayerData.Instance.GetCurrentChallengeLevel()==1)
			{
				GameController.Instance.PauseGame();
				UIGuideControllor.Instance.Show(UIGuideType.GamePlayerUILeftGuide);
				UIGuideControllor.Instance.ShowBubbleTipByID(8);
			}
			break;
		case GameGuideStep.Right:
			if(PlayerData.Instance.GetCurrentChallengeLevel()==1)
			{
				GameController.Instance.PauseGame();
			   UIGuideControllor.Instance.Show(UIGuideType.GamePlayerUIRightGuide);
				UIGuideControllor.Instance.ShowBubbleTipByID(9);
			}
			break;
		case GameGuideStep.UseCurProp:
			if(PlayerData.Instance.GetCurrentChallengeLevel()==1)
			{
				GameController.Instance.PauseGame();
			    UIGuideControllor.Instance.Show(UIGuideType.GamePlayerUIUseCurPropGuide);
				UIGuideControllor.Instance.ShowBubbleTipByID(10);
			}
			break;
		case GameGuideStep.UseSpeedup:
			if(PlayerData.Instance.GetCurrentChallengeLevel()==2  && PlayerData.Instance.GetItemNum(PlayerData.ItemType.SpeedUp)>0)
			{
				GameController.Instance.PauseGame();
			    UIGuideControllor.Instance.Show(UIGuideType.GamePlayerUIUseSpeedupGuide);
				UIGuideControllor.Instance.ShowBubbleTipByID(12);
			}
			break;
		case GameGuideStep.UseFlyBmob:
			if(PlayerData.Instance.GetCurrentChallengeLevel()==2  && PlayerData.Instance.GetItemNum(PlayerData.ItemType.FlyBomb)>0)
			{
				GameController.Instance.PauseGame();
			    UIGuideControllor.Instance.Show(UIGuideType.GamePlayerUIUseFlyBmobGuide);
				UIGuideControllor.Instance.ShowBubbleTipByID(11);
			}
			break;
		}
	
		gameObject.SetActive (false);
		CollectInfoEvent.SendEvent (CollectInfoEvent.EventType.Guide, "Step",curStep.ToString());
	}
}
