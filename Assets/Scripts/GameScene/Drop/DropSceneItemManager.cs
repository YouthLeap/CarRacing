using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using DG.Tweening;

/// <summary>
/// 游戏中的掉落控制 
//掉落金币控制
/// </summary>
public class DropSceneItemManager : MonoBehaviour {

	private SpawnPool pool;

	private static DropSceneItemManager _instance=null;
	public static DropSceneItemManager Instance
	{
		get{
			if(_instance == null)
			{
				GameObject go = new GameObject();
				go.name = "DropSceneItemManager";
				_instance = go.AddComponent<DropSceneItemManager>();
			}
			return _instance;
		}
	}


	void OnEnable()
	{
		if (pool == null) {
			pool = PoolManager.Pools["ItemPool"];
		}
	}

	/// <summary>
	/// Boss 爆炸金币
	/// </summary>
	/// <param name="bossTran">Boss tran.</param>
	public void BossExplodeCoin(Transform bossTran)
	{
		StartCoroutine (IEBossExplodeCoin(bossTran));
	}

	IEnumerator IEBossExplodeCoin(Transform bossTran)
	{
		float radius = 3f;
		List<Transform> coinList = new List<Transform> ();
		for (int i=0; i<11; ++i) {
			Transform coinTran =pool.Spawn("CoinProp");
			coinTran.position = bossTran.position + new Vector3(0,4,0);
			
			float x= radius*Mathf.Cos(i*36);
			float y= radius*Mathf.Sin(i*36);
			Vector3 pos = coinTran.position;
			pos.x +=x;
			pos.y += y;
			
			coinTran.DOMove(pos,0.4f);
			coinList.Add(coinTran);
		}
		
		yield return new WaitForSeconds(0.9f);
//		for (int j=0; j<coinList.Count; ++j) {
//			coinList[j].GetComponent<CoinProp>().UseAbsorbAnim(PlayerController.Instance.transform ,0f,4f);
//		}
	}

	/// <summary>
	/// BOSS 死翘翘了 ,然后爆金币
	/// </summary>
	/// <param name="bossTran">Boss tran.</param>
	public void BossDeadExplodeCoin(Transform bossTran)
	{
		StartCoroutine (IEBossDeadExplodeCoin(bossTran));
	}

	IEnumerator IEBossDeadExplodeCoin(Transform bossTran)
	{
		float startZ = bossTran.position.z;
		float zInternal = 4f;

		List<Transform> coinList = new List<Transform> ();

		for (int i=0; i<5; ++i) {
			for(int j=0;j<6;++j)
			{
				Transform coinTran =pool.Spawn("CoinProp");

				coinTran.position = bossTran.position + new Vector3(0,10f,0);
				//coinTran.parent = ItemCreateManager.Instance.frontGroup;
				coinList.Add(coinTran);
				
				float x = 17.5f * 0.5f * GlobalConst.lanes[i];
				float z = startZ + j* zInternal;
	
				coinTran.position =new Vector3(x,8f+Random.Range(1f,5f) ,z+Random.Range(0,2f));

				Vector3 targetPos = new Vector3(x+Random.Range(0,0.5f),1f,z+Random.Range(0,2f));
				coinTran.DOMove(targetPos,0.4f);

				yield return new WaitForEndOfFrame();
			}
		}

	   //yield return new WaitForSeconds(0.8f);

//		for (int k=0; k<coinList.Count; ++k) {
//		    if(coinList[k]!=null && coinList[k].gameObject.activeSelf)
//			{
//				coinList[k].GetComponent<CoinProp>().UseAbsorbAnim(PlayerController.Instance.transform,0f,3f);
//				yield return new WaitForEndOfFrame();
//			}
//		}

	}


}
