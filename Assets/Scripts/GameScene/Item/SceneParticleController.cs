using UnityEngine;
using System.Collections;
using PathologicalGames;

public class SceneParticleController : MonoBehaviour {
	public static SceneParticleController Instance;
	private SpawnPool particelPool;

	private int particleCount=0;

	void Awake()
	{
		Instance = this;

	}
	// Use this for initialization
	void Start () {
		particelPool = PoolManager.Pools["SceneParticlePool"];
	}
	
	
	public void PlayParticle(string particeName, Vector3 pos,Transform parentTran = null,float delay =0f)
	{
		/*低端手机的粒子特效控制 限制粒子的数量*/
		if (QualitySetting.deviceQualityLevel == DeviceQualityLevel.Low   && particleCount >12) {
			return;
		}

		Transform particle = particelPool.Spawn (particeName);
		
		if (particle == null) {
			return;
		}

		if (parentTran == null) {
			particle.parent = this.transform;
		} else {
			particle.parent = parentTran;
		}

		particle.position = pos;
		++particleCount;
		StartCoroutine (DelayRecycle (particle,delay));
	}
	
	
	IEnumerator DelayRecycle(Transform parti,float delay)
	{
		yield return new WaitForSeconds (delay);
		
		if (parti.GetComponent<ParticleSystem> () != null) {
			parti.GetComponent<ParticleSystem> ().Play (true);
		}
		
		yield return new WaitForSeconds (delay+2f);
		particelPool.Despawn (parti);
		--particleCount;
	}
}
