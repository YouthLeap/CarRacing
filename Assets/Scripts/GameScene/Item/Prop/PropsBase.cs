using UnityEngine;
using System.Collections;
using PathologicalGames;
using DG.Tweening;


/// <summary>
/// 所有道具的基类.
/// </summary>
public class PropsBase : ItemBase {
	/// <summary>
	/// 是否可以被磁铁吸收.
	/// </summary>
	public bool isCanBeAbsorb = true;
	
	public override void Init ()
	{
		base.Init ();
		pool = PoolManager.Pools["ItemPool"];

	

	}
	
	public override void GetItem (PropControl  propCon= null)
	{
		if(pool == null)
		{
			pool = PoolManager.Pools["ItemPool"];
		}

		if (gameObject.activeInHierarchy) {
			gameObject.SetActive(false);
		}
	}
	
	/// <summary>
	/// 使用磁铁吸收的效果.
	/// </summary>
	/// <param name="target">Target.</param>
	public  void UseAbsorbAnim(Transform target,float delay=0f,float offsetZ= 1f)
	{
		if(!isCanBeAbsorb)
			return;

		if(gameObject.activeInHierarchy)
			StartCoroutine (UseAbsorb(target,delay,offsetZ));
	}

	//Magnet method
	IEnumerator UseAbsorb(Transform target,float delay =0f,float offsetZ=1f){

		if (delay > 0) {
			yield return new WaitForSeconds(delay);
		}

		Vector3 offsetVec = new Vector3(0f,1.5f,2f);


		bool isLoop = true;
		float countTime = 0;
		isUseAbsorbing = true;
		while(isLoop)
		{
			countTime += Time.deltaTime;
			this.transform.position = Vector3.Lerp(this.transform.position, target.position + offsetVec, 10*Time.deltaTime);

			float distance =Vector3.Distance(this.transform.position, target.position+  offsetVec ) ;

			//Debug.Log("isLoop "+distance);

			if(distance<0.8f || countTime > 0.3f){
				isLoop = false;
				isUseAbsorbing = false;
				GetItem();
			}
			yield return 0;
		}

		StopCoroutine("UseAbsorb");
		yield return 0;
	}

	public override void Reset ()
	{
		base.Reset ();
	}

	
	protected void AddValueByID()
	{
		if (id == 0) {
			return;
		}

		GameData.Instance.curCoin+= PropConfigData.Instance.GetAddCoin(id) * GameData.Instance.curMulti;
	}
	
}
