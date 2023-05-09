using UnityEngine;
using System.Collections;

/// <summary>
/// 金币道具
/// </summary>
public class CoinProp : PropsBase {

	public enum CoinType
	{
		Big,
		Small
	}

	public CoinType coinType;

	public override void Init ()
	{
		base.Init ();
	}


	public override void GetItem (PropControl  propCon= null)
	{

		if(propCon== null)
		{
			return;
		}

		base.GetItem ();
		Vector3 pos =propCon.transform.position + propCon.transform.forward*3.5f;
		SceneParticleController.Instance.PlayParticle ("FX_jinbi01_hg",pos,propCon.transform);

		if(propCon.isPlayer == false)
		{
			return;
		}

		AddValueByID();
		DropItemManage.Instance.DropCoin(transform.position,1);

		if (coinType == CoinType.Big) {
			AudioManger.Instance.PlaySound (AudioManger.SoundName.GetCoin);
		} else {
			AudioManger.Instance.PlaySound(AudioManger.SoundName.Get);
		}
	}
}