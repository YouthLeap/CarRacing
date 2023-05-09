using UnityEngine;
using System.Collections;


/// <summary>
/// 活动彩蛋
/// </summary>
public class ColorEggProp : PropsBase {

    public override void Init ()
	{
		base.Init ();

		if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe) {
			gameObject.SetActive(false);
			return;
		}

		if (ExchangeActivityData.Instance.GetActivityEnable() == false) {
			gameObject.SetActive(false);
			return;
		}

		if (Random.value > ExchangeActivityData.Instance.GetProbability()) {
			gameObject.SetActive(false);
			return;
		}
	}

	public override void GetItem (PropControl  propCon= null)
	{
		base.GetItem ();

		GameData.Instance.curEggCount++;

		AudioManger.Instance.PlaySound (AudioManger.SoundName.GetCoin);

		DropItemManage.Instance.ShowGetColorEgg ();
	}
}
