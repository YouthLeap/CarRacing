using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;

/// <summary>
/// 角色特效控制
/// </summary>
public class CarParticleControl : MonoBehaviour {

	private SpawnPool carParticlePool;
	private Transform particleTran;
	private ParticleSystem particlePS;
	private Hashtable particleTable;
	private CarMove carMove;
	private int carId;

	public List<GameObject> traceList= new List<GameObject>();
	public List<GameObject> sparkList = new List<GameObject>();
	[HideInInspector]
	public bool isShowRunEffect=false;
	private Vector3[] effectOfsetVec=new Vector3[5];

	void Start()
	{
		carParticlePool = PoolManager.Pools ["CarParticlePool"];
		carMove = GetComponent<CarMove>();
	

		effectOfsetVec[0]=new Vector3(0,-0.56f,-0.28f);
		effectOfsetVec[1]=new Vector3(0,-0.45f,-0.2f);
		effectOfsetVec[2]=new Vector3(0f,-0.8f,-0.4f);
		effectOfsetVec[3]=new Vector3(0,-0.7f,-0.35f);
		effectOfsetVec[4]=new Vector3(0f,0f,0f);

		Invoke("LoadCarRunParticle",0.1f);
	}


	public void ShowTrace()
	{
		for(int i=0;i<traceList.Count;++i)
		{
			traceList[i].SetActive(true);
		}

		isShowRunEffect=true;
	}
	public void HideTrace()
	{
		for(int i=0;i<traceList.Count;++i)
		{
			traceList[i].SetActive(false);
		}
		isShowRunEffect=false;
	}

	public void ShowSpark()
	{
		for(int i=0;i<this.sparkList.Count;++i)
			sparkList[i].SetActive(true);
	}
	public void HideSpark()
	{
		for(int i=0;i<this.sparkList.Count;++i)
			sparkList[i].SetActive(false);
	}

	void LoadCarRunParticle()
	{
		Transform carTran= GetComponent<CarMove>().carTransform;
		carId = carMove.carId;

		PlayerWheelsRotation[] wheelArray=  GetComponentsInChildren<PlayerWheelsRotation>();
		//Debug.Log(wheelArray.Length.ToString());
		if(wheelArray.Length!=4)
		{
			return;
		}

		int index = IDTool.GetModelType(carId)-1;
		Vector3 offset= effectOfsetVec[index];

		traceList.Clear();
		for(int i=0;i<4;++i)
		{
		   GameObject 	traceGO=    carParticlePool.Spawn("FX_taihen01").gameObject;
			traceGO.transform.parent =carTran;
			traceGO.SetActive(false);
			traceList.Add(traceGO);

			traceGO.transform.position = wheelArray[i].transform.position+offset;
		}

//		traceList[0].transform.localPosition = new Vector3(-0.8f,0,-0.8f);
//		traceList[1].transform.localPosition = new Vector3(0.8f,0,-0.8f);
//		traceList[2].transform.localPosition= new Vector3(-0.8f,0,1.1f);
//		traceList[3].transform.localPosition = new Vector3(0.8f,0,1.1f);


		sparkList.Clear();
		for(int j=0;j<4;++j)
		{
			GameObject spark = carParticlePool.Spawn("FX_huohua02").gameObject;
			spark.transform.parent = carTran;
			spark.SetActive(false);
			sparkList.Add(spark);
			spark.transform.position = wheelArray[j].transform.position +offset;
		}
	}


	public void Init()
	{
		carParticlePool = PoolManager.Pools ["CarParticlePool"];
		particleTable = new Hashtable ();
	}

	public void PlayParticle(CarParticleType particleType, bool autoStop = false, float waitTime = 0)
	{
		if (particleTable.ContainsKey (particleType))
			return;

		switch (particleType) {
		case CarParticleType.SpeedUp:
			particleTran = carParticlePool.Spawn ("FX_wudijiasu01_hg", transform.parent);
			break;
		case CarParticleType.Shield:
			particleTran = carParticlePool.Spawn ("FX_hudun02", transform.parent);
			break;
		case CarParticleType.Magnet:
			particleTran = carParticlePool.Spawn ("FX_citie01_hg", transform.parent);
			break;
		case CarParticleType.Lighting:
			particleTran = carParticlePool.Spawn ("FX_shandian01_hg", transform.parent);
			break;
		case CarParticleType.Vertigo:
			particleTran = carParticlePool.Spawn ("FX_xuanyun01_hg", transform.parent);
			break;
		case CarParticleType.Mushroom:
			particleTran = carParticlePool.Spawn ("FX_bianda01_hg", transform.parent);
			break;

		case CarParticleType.ShapeSpeedUp:
			particleTran = carParticlePool.Spawn ("FX_kaixinjiasu01_hg", transform.parent);
			break;

		case CarParticleType.RandomProp:
			particleTran = carParticlePool.Spawn ("FX_chidaoju01_hg", transform.parent);
			break;

		case CarParticleType.Attack:
			particleTran = carParticlePool.Spawn ("FX_kaixingongji_hg", transform.parent);
			break;

		case CarParticleType.AttackBoBi:
			particleTran = carParticlePool.Spawn ("FX_bobibianshen01", transform.parent);
			break;

		case CarParticleType.AttackExplode:
			particleTran = carParticlePool.Spawn ("FX_baozha01_hg", transform.parent);
			break;
		}



		particleTran.localScale = Vector3.one * transform.localScale.x;
		particleTran.parent = transform;

	
		switch(particleType)
		{
		case CarParticleType.ShapeSpeedUp:
			if(IDTool.GetModelType(carId) == 2 || IDTool.GetModelType(carId) == 5)
			{
				particleTran.localPosition = CarManager.Instance.carBackOffset+new Vector3(0f,4.5f,0);
			}else
			{
				particleTran.localPosition = CarManager.Instance.carBackOffset;
			}
			break;
		default:
			particleTran.localPosition = CarManager.Instance.carBackOffset;
			break;
		}

		particleTran.localEulerAngles = Vector3.zero;
		particleTran.gameObject.SetActive (true);
		particlePS = particleTran.GetComponent<ParticleSystem> ();
		particlePS.Play ();

		particleTable.Add (particleType, particleTran);

		if (autoStop) {
			StartCoroutine (IEStopParticle (particleType, waitTime));
		}
	}


	public void StopParticle(CarParticleType particleType)
	{
		if (particleTable.ContainsKey (particleType) == false)
			return;
		particleTran = (Transform)particleTable [particleType];
		particlePS = particleTran.GetComponent<ParticleSystem> ();
		particlePS.Stop ();
		particleTran.gameObject.SetActive (false);

		carParticlePool.Despawn (particleTran);
		particleTable.Remove (particleType);
	}

	private IEnumerator IEStopParticle(CarParticleType particleType, float waitTime)
	{
		yield return new WaitForSeconds (waitTime);
		StopParticle (particleType);
	}
}