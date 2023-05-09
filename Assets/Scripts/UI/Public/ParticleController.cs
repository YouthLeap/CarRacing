using UnityEngine;
using System.Collections;
using PathologicalGames;

public enum ParticleType
{
	ButtonClick,
	LevelUpgrade,
	SelectPlayer,
};
public class ParticleController : MonoBehaviour {

	public static ParticleController Instance;

	private SpawnPool particlePool;

	void Awake ()
	{
		Instance = this;
	}

	void Start ()
	{
		particlePool = PoolManager.Pools["UIParticlePool"];
	}
	
	public void PlayEffect (ParticleType particleType, Vector3 position)
	{
		if (QualitySetting.deviceQualityLevel == DeviceQualityLevel.Low)
			return;
		Transform particleTran = particlePool.Spawn (particleType.ToString ());
		particleTran.position = position;
		particleTran.GetComponent<ParticleSystem> ().Play ();
		StartCoroutine (StopEffect (particleTran));
	}

	IEnumerator StopEffect(Transform particleTran)
	{
		yield return new WaitForSeconds (3.0f);
		particlePool.Despawn (particleTran);
	}
}