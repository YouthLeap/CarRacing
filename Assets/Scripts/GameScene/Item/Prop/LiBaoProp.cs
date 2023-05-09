using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LiBaoProp : PropsBase {

	public static bool isShow=false;

	public Transform[] modelArray;
	public override void Init ()
	{
		base.Init ();

		if (PlayerData.Instance.IsWuJinGameMode ()) {
			gameObject.SetActive(false);
			return;
		}
		if(!LevelData.Instance.GetYizeGiftState(PlayerData.Instance.GetSelectedLevel()) && !LevelData.Instance.GetFreeYizeGiftState(PlayerData.Instance.GetSelectedLevel()))
		{
			gameObject.SetActive(false);
			return;
		}

		for(int i=0;i<modelArray.Length;++i)
		{
			modelArray[i].DOKill();
			modelArray[i].DORotate(new Vector3(0,180,0),2f).SetLoops(-1,LoopType.Incremental).SetEase(Ease.Linear);
		}

		if(isShow)
		{
			gameObject.SetActive(false);
			return;
		}
		isShow = true;
	}

	public override void GetItem (PropControl  propCon= null)
	{

		if(propCon==null )
			return;

		if(propCon.isPlayer==false)
			return;

		base.GetItem ();

		GameController.Instance.PauseGame();

		if (LevelData.Instance.GetFreeYizeGiftState (PlayerData.Instance.GetSelectedLevel ())) {
			LevelGiftControllor.Instance.Show(PayType.FreeInnerGameGift,Complete, true);
			//CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_Light,"State","自动弹出","Level",PlayerData.Instance.GetSelectedLevel().ToString());
		} else {
			LevelGiftControllor.Instance.Show(PayType.InnerGameGift,Complete, true);
			CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Gift_Light,"State","自动弹出","Level",PlayerData.Instance.GetSelectedLevel().ToString());
		}
	}

	private void Complete()
	{
		PlayerCarControl.Instance.propCon.UsePropByType(PropType.FlyBmob);
	}
}
