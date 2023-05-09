using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// 竞争对手的车
/// </summary>
[RequireComponent(typeof(CarMove))]
[RequireComponent(typeof(PropControl))]
[RequireComponent(typeof(CarParticleControl))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class OpponenetCarControl : MonoBehaviour {

	public bool isDebug=false;

	public Transform selfTrans;

	public CarMove carMove;
	public PropControl propCon;
	public CarParticleControl carParticleCon;
	
	private  Vector3 rayCheckUpOffset= new Vector3(0,0.8f,0);
	private int checkForwarCount=0;

	public void Init()
	{
		selfTrans = transform;

		carMove = GetComponent<CarMove> ();
		propCon = GetComponent<PropControl> ();
		carParticleCon = GetComponent<CarParticleControl> ();

		propCon.Init ();
		carParticleCon.Init ();

		StartCoroutine(IERandomChangeOffset());
		carMove.StartRun();

	}

	void FixedUpdate()
	{
		CheckForwadCar();
	}

	void Update () {
	}

	/// <summary>
	/// Checks the forwad car.
	/// 自动躲避前面的车
	/// </summary>
	void CheckForwadCar()
	{

		++checkForwarCount;
		if(checkForwarCount<4)
			return;
		checkForwarCount=0;

		if(carMove.isChangeXOffset)
		{
			return;
		}

		bool leftForwar  = Physics.Raycast(selfTrans.position+rayCheckUpOffset-selfTrans.right*0.7f,selfTrans.forward,10,1<<LayerMask.NameToLayer("ItemCar"));
		bool rightForwar  = Physics.Raycast(selfTrans.position+rayCheckUpOffset+selfTrans.right*0.7f,selfTrans.forward,10,1<<LayerMask.NameToLayer("ItemCar"));

		if(leftForwar || rightForwar)
		{
			ChangeOffsetRand(30f);
		}
	}

	/// <summary>
	/// 随机改变偏移量
	/// </summary>
	/// <returns>The random change offset.</returns>
	IEnumerator IERandomChangeOffset()
	{
		yield return new WaitForSeconds(6f);

		while(true)
		{
			float randTime= Random.Range(3f,6f);
			yield return new WaitForSeconds(randTime);

			//Debug.Log("IERandomChangeOffset");
			if(carMove.isChangeXOffset == false)
			{
				ChangeOffsetRand();
			}
			   
		}
	}

	void  ChangeOffsetRand(float changeSpeed=5f)
	{
	
		//check left and right side
		bool rightRay= Physics.Raycast(transform.position+rayCheckUpOffset,selfTrans.right,5f);
		bool leftRay= Physics.Raycast(transform.position+ rayCheckUpOffset, -selfTrans.right,5f);
		
		float changeOffset=0;
		float changeValue=Random.Range(2,5);
		if(!leftRay && !rightRay)
		{
			int d = Random.value>0.5?1:-1;
			changeOffset = d* changeValue;
		}else if(!leftRay)
		{
			changeOffset = changeValue;
		}else if(!rightRay)
		{
			changeOffset = -changeValue;
		}

		if(isDebug)
		{
		   Debug.Log("checkOffsetRand "+leftRay+" "+rightRay);
		}

		//change offset
		if(changeOffset !=0)
		{
			float targetOffset=	carMove.xOffset+changeOffset ;
			carMove.ChangeOffsetTo(targetOffset);
		}
	}
}