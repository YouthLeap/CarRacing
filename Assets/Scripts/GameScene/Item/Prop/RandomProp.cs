using UnityEngine;
using System.Collections;

public class RandomProp : PropsBase {

	private CarParticleControl carParticleCon;

	public int propId=0;

	public override void Init ()
	{
		base.Init ();

		for(int i=0;i<this.transform.childCount;++i)
		{
			transform.GetChild(i).gameObject.SetActive(true);
		}

		StopCoroutine("IEReset");
	}

	public override void GetItem (PropControl propCon)
	{
		if(propCon == null)
		{
			return;
		}
		//base.GetItem (propCon);
		for(int i=0;i<this.transform.childCount;++i)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}

		if(propId!=0)
		{
			id=propId;
		}else
		{
			id = DropItemManage.Instance.GetRandomPropId (propCon.isPlayer);
		}
		propCon.GetProp(id);

		carParticleCon = propCon.carMove.carParticleCon;
		carParticleCon.PlayParticle (CarParticleType.RandomProp, true, 2.0f);

		if (propCon.isPlayer)
			AudioManger.Instance.PlaySound (AudioManger.SoundName.GetScore);


		StartCoroutine("IEReset");
	}

	IEnumerator IEReset()
	{
		float  cal=0;
		while(cal<3f)
		{
			cal+=Time.deltaTime;
			yield return null;
			while(GameData.Instance.IsPause)
			{
				yield return null;
			}
		}

		for(int i=0;i<this.transform.childCount;++i)
		{
			transform.GetChild(i).gameObject.SetActive(true);
		}
	}
}