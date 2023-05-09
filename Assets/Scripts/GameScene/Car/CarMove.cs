using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 车的移动控制
/// </summary>
//[ExecuteInEditMode]
public class CarMove : MonoBehaviour {

	public int carId;
	public bool isPlayer=false;
	public bool isDebug=false;
	public bool applyStartPos=false;
	public Transform carTransform=null;

	public float  startPosLen=0f; //start position
	public float xOffset;	

	public enum CarType{ PlayerCar,OpponentCar,NormalCar};
	public CarType carType=CarType.NormalCar;
	public float explodeSpeed=10f;
	public PropControl propCon;
	public CarParticleControl carParticleCon;
	public AnimatorManager animManager;
	public MechaAnimatorManager mechaAnimManager;

	public float speed=1f;
	public float acceleration=0.1f;
	public float maxSpeed=30f;
	public float origMaxSpeed=30f;

	public bool isFinish=false;
	public float runningTime=0;
	public float moveLen=0; // car position in the path

	public bool isTurnLeft=false;
	public bool isTurnRight=false;
	public float yMaxRotate;
	
	public bool isRunning=false;

	public int circleCount=0;/*圈数*/
	
	[HideInInspector]
	public float minXOffset=-18f;
	[HideInInspector]
	public float maxXOffset=18f;

	RoadPathManager roadPath;


	private Transform selfTrans;
	private List<Vector3> pointList;
	private List<Vector3> origPointList;
	private Vector3 prePos;
	private Rigidbody rigidBody;

	private bool isLefting=false;
	private bool isRighting=false;

	float leftRightChange=0.6f;
	
	public void  SetDataById(int id)
	{
		carId = id;
		if(isPlayer)
		{
			this.maxSpeed = ModelData.Instance.GetRunSpeed(id);
			this.origMaxSpeed = maxSpeed;
			this.acceleration = ModelData.Instance.GetAcceleration(id);
		}else
		{
			this.maxSpeed = ModelData.Instance.GetOppRunSpeed(id);
			this.origMaxSpeed = maxSpeed;
			this.acceleration = ModelData.Instance.GetOppAcceleration(id);
		}
	}
	
	public float SetMaxSpeed()
	{
		if (propCon.isSpeedUp)
			maxSpeed = origMaxSpeed * 2.5f;
		else if (propCon.isShapeShift)
			maxSpeed = origMaxSpeed * 1.5f;
		else if (propCon.isSpeedAdd)
			maxSpeed = origMaxSpeed * 1.5f;
		else if (propCon.isMushroom)
			maxSpeed = origMaxSpeed * 1.5f;
		else
			maxSpeed = origMaxSpeed;

		if (propCon.isPlayer) {
			if(maxSpeed > origMaxSpeed)
				CarCameraFollow.Instance.SetSpeedUpEffect (true);
			else
				CarCameraFollow.Instance.SetSpeedUpEffect (false);
		}

		return maxSpeed;
	}

	public void StartRun()
	{
		propCon = GetComponent<PropControl>();
		carParticleCon = GetComponent<CarParticleControl>();
		
		roadPath = RoadPathManager.Instance;
		rigidBody = GetComponent<Rigidbody>();
		GetComponent<BoxCollider>().isTrigger= true;
		
		origPointList=roadPath.origPointList;
		pointList = roadPath.smoothPointList;
		
		moveLen = startPosLen;
		isRunning = true;
		circleCount = 1;
		selfTrans = this.transform;

		rigidBody.isKinematic = true;
		Vector3 pos=  RoadPathManager.Instance.GetPointByLen(startPosLen,xOffset);
		Vector3 ahead=  RoadPathManager.Instance.GetPointByLen(startPosLen+2f,xOffset);
		prePos=pos;
		transform.position= pos;
		selfTrans.LookAt(ahead);
		rigidBody.useGravity = false;

		preCenterPos = pos;
		curCenterPos = ahead;
		curCenterMoveDir = curCenterPos - preCenterPos;
	}

	void Update()
	{
		if(applyStartPos)
		{
		     Vector3 pos=  RoadPathManager.Instance.GetPointByLen(startPosLen,xOffset);
			Vector3 ahead=  RoadPathManager.Instance.GetPointByLen(startPosLen+2f,xOffset);
			transform.position= pos;
			transform.LookAt(ahead);
			applyStartPos=false;
		}
	}

	// Update is called once per frame
	void FixedUpdate () {

		if(GameData.Instance.IsPause)
		{
			return;
		}


		if(moveLen >= CarManager.Instance.totalPathLen)
		{
			isFinish=true;
		}
		if(isFinish==false)
		{
		  runningTime+=Time.fixedDeltaTime;
		}

		if(isRunning == false)
			return;

		if(isTurnRight)
		{
			if(xOffset>minXOffset+0.5)
			{
				float targetOffset=xOffset-leftRightChange;
				ChangeOffsetTo(targetOffset);
				if(yMaxRotate < 45)
					yMaxRotate += 5;
			}
		}
		else if(isTurnLeft)
		{
			if(xOffset<maxXOffset-0.5)
			{
				float targetOffset=xOffset+leftRightChange;
				ChangeOffsetTo(targetOffset);
				if(yMaxRotate > -45)
					yMaxRotate -= 5;
			}
		}
		
		if (!isTurnRight && !isTurnLeft)
		{
			if (yMaxRotate > 0)
				yMaxRotate -= 3;
			else if (yMaxRotate < 0)
				yMaxRotate += 3;
		}

		//change speed
		if(speed <= maxSpeed)
		{
			speed+=acceleration;
			speed = speed>maxSpeed?maxSpeed:speed;
		}else{
			speed-=acceleration*3f;
			speed= speed<maxSpeed?maxSpeed:speed;
		}
		MoveToNextPoint();
		UpdateOffsetChange();
	}


	#region  移动控制
	const float rotateTimeScale=8f;
	const float speedUpLimit=75;//
	float changeAngle=0f;
	float maxChangeAngle=40;/*最大转弯角度*/

	Vector3 forwarCenterPos = Vector3.zero;
	Vector3 curCenterPos=Vector3.zero;
	Vector3 preCenterPos= Vector3.zero;

	private  float turnSpeed=20;/*转弯速度*/
	private float turnSpeedAcc=3;/*转弯加速度*/
	private float turnSpeedMin=30;
	private float turnSpeedMax=100;

	public Vector3 curCenterMoveDir= Vector3.zero;
	public Vector3 carMoveDir= Vector3.forward;
	private Quaternion  curMoveQua=Quaternion.identity;
	private bool isPreShowDriftingEffect=false;
	/// <summary>
	/// 赛车的移动控制
	/// </summary>
	void MoveToNextPoint()
	{
		moveLen+=speed * Time.fixedDeltaTime;
		selfTrans.position = roadPath.GetPointByLen(moveLen,xOffset);
		forwarCenterPos = roadPath.GetPointByLen(moveLen+25f);
		curCenterPos = roadPath.GetPointByLen(moveLen);
		curCenterMoveDir =  forwarCenterPos-curCenterPos; /*道路中心的方向向量*/
		/*车移动的方向向量*/
		carMoveDir = (roadPath.GetPointByLen(moveLen+25f,xOffset) - prePos).normalized;
		prePos = selfTrans.position;
		/*切道控制*/
		if(isChangeXOffset && isChangeSpeedUp )
		{
			if(isLefting)
				changeAngle-=turnSpeed*Time.deltaTime;
		    else
				changeAngle+=turnSpeed*Time.deltaTime;
			changeAngle = Mathf.Clamp(changeAngle,-maxChangeAngle,maxChangeAngle);

			turnSpeed+=turnSpeedAcc;
			turnSpeed= Mathf.Min(turnSpeed,turnSpeedMax);

			curMoveQua = Quaternion.LookRotation(carMoveDir);
			Vector3 er=curMoveQua.eulerAngles;
			er.y+=changeAngle;
			curMoveQua.eulerAngles=er;
			selfTrans.rotation= curMoveQua;

			ShowDriftEffect();
		}
		/*切道恢复*/
		else if(Mathf.Abs(changeAngle)>turnSpeed*Time.deltaTime)
		{
			if(changeAngle>0)
				changeAngle-=turnSpeed*Time.deltaTime;
			else
				changeAngle+=turnSpeed*Time.deltaTime;
			changeAngle = Mathf.Clamp(changeAngle,-maxChangeAngle,maxChangeAngle);

			curMoveQua = Quaternion.LookRotation(carMoveDir);
			Vector3 er=curMoveQua.eulerAngles;
			er.y+=changeAngle;
			curMoveQua.eulerAngles=er;
			selfTrans.rotation= curMoveQua;

			HideDriftEffect();
		}
		/*按照道路移动*/
		else
		{
			changeAngle=0;
			turnSpeed= turnSpeedMin;
			selfTrans.forward = Vector3.Lerp(selfTrans.forward, carMoveDir,rotateTimeScale*Time.deltaTime);
		}

		if(moveLen > circleCount*roadPath.pathLen)
		{
			circleCount++;
		}

		if(isDebug)
		{
			Debug.DrawRay(selfTrans.position,selfTrans.forward*10,Color.red);
		}
	}

	private void ShowDriftEffect()
	{
		if(isPlayer==false)
			return;

		if( Mathf.Abs(changeAngle)>15 && carParticleCon!=null && !isCarExplode & !isSliping)
		{
			carParticleCon.ShowSpark();
			for(int n=0;n<carParticleCon.sparkList.Count;++n)
			{
				carParticleCon.sparkList[n].transform.forward= curCenterMoveDir;
			}
		}
		
		if( Mathf.Abs(changeAngle)>maxChangeAngle-10 && carParticleCon!=null)
		{
			carParticleCon.ShowTrace();
			for(int k=0;k<carParticleCon.traceList.Count;++k)
			{
				carParticleCon.traceList[k].transform.forward= - curCenterMoveDir;
			}
			
			if(propCon.isShapeShift==false)
			{
				AudioManger.Instance.PlayDriftSound(AudioManger.SoundName.Drifting);
				CarCameraFollow.Instance.SetSpeedUpEffect(true);
			}
		}
	}

	private void HideDriftEffect()
	{
		if(isPlayer==false)
			return;

		if(carParticleCon!=null)
		{
			carParticleCon.HideSpark();
			carParticleCon.HideTrace();

			AudioManger.Instance.StopDriftSound(AudioManger.SoundName.Drifting);
			if(!propCon.isSpeedAdd && !propCon.isSpeedUp)
			{
				CarCameraFollow.Instance.SetSpeedUpEffect(false);
			}
		}
	}

	#endregion

	void  OnDrawGizmos()
	{
		iTweenPath.DrawVirtualPath(pointList);
	}

	#region 碰撞处理

	public bool IsInvincible()
	{
		if( propCon!=null && (propCon.isShapeShift || propCon.isMushroom || propCon.isSpeedUp ||propCon.isShield))
			return true;

		return false;
	}

	CarMove otherCar;
	void OnTriggerEnter(Collider other) {
		if(other.CompareTag("ItemCar") == false)
			return;
		
		if ((otherCar =other.GetComponent<CarMove>()) == null)
		{
			return;
		}
		if(isRunning == false)
		{
			return ;
		}

		//判断后面碰撞
		bool backCollision = ( otherCar.moveLen - this.moveLen)>1?true:false;

		if(backCollision)
		{
			if(otherCar.carType == CarType.NormalCar && IsInvincible())
			{
				otherCar.ExplodeCar();

				if(isPlayer)
				{
					AudioManger.Instance.PlaySound (AudioManger.SoundName.CarCrashFly);
					GameData.Instance.crashCarCount++;
					otherCar.GetComponent<NormalCarItem>().AddCoinByID();
				}

			}else if(otherCar.carType == CarType.NormalCar && Mathf.Abs(this.speed - this.maxSpeed) <0.1f )
			{
				otherCar.ExplodeCar();
				this.speed *=0.8f;

				if(isPlayer)
				{
					AudioManger.Instance.PlaySound (AudioManger.SoundName.CarCrashFly);
					GameData.Instance.crashCarCount++;
					otherCar.GetComponent<NormalCarItem>().AddCoinByID();
				}
			}
			else
			{
				otherCar.speed+=20f;

				if(IsInvincible() == false)
				{
				    this.speed = otherCar.speed*0.6f;
				}
				
				if(xOffset>=otherCar.xOffset)
				{
					ChangeOffsetTo(xOffset+2,true);
				}else
				{
					ChangeOffsetTo(xOffset-2,true);
				}

				if(isPlayer)
				{
					GameData.Instance.crashCount++;
				}

				if (isPlayer || otherCar.isPlayer) {
					AudioManger.Instance.PlaySound (AudioManger.SoundName.CarCrash);
				}
			}

			SceneParticleController.Instance.PlayParticle("explosion",selfTrans.position+selfTrans.forward*3f+selfTrans.up,selfTrans);
		}
		else
		{
			float changeX=2f;
			if(otherCar.IsInvincible())
			{
				changeX=3.3f;
			}

			if ( this.xOffset < otherCar.xOffset )
			{
				float targetOffset = xOffset-changeX;
				ChangeOffsetTo(targetOffset,true);
				SceneParticleController.Instance.PlayParticle("explosion",selfTrans.position-selfTrans.right*1.8f+selfTrans.up,selfTrans);
			}
			else if( this.xOffset > otherCar.xOffset)
			{
				float targetOffset = xOffset+changeX;
				ChangeOffsetTo(targetOffset,true);
				SceneParticleController.Instance.PlayParticle("explosion",selfTrans.position+selfTrans.right*1.8f+selfTrans.up,selfTrans);
			}
		}

		
	}
		
	public bool isChangeXOffset=false;
	float targetOffset=0f;
	float targetOffsetEnd=0f;
	float changeSpeed=0f;
	float changeSpeedAcc=1f;
	float maxChangeSpeed=25f;
	float minChangeSpeed=5f;
	float explodeChangeSpeed=35f;
	bool isLockChange=false;
	bool isChangeSpeedUp=false;

	float maxChangeSpeedUp=25f;
	float maxChangeSpeedDown=10f;

	/// <summary>
	/// 左右切道控制
	/// </summary>
	/// <param name="targetOffset">Target offset.</param>
	/// <param name="isLock">If set to <c>true</c> is lock.</param>
	public void ChangeOffsetTo(float targetOffset, bool isLock=false)
	{
		if(isLockChange)
		{
			return;
		}

		isChangeXOffset =true;
		this.targetOffset = Mathf.Clamp(targetOffset,minXOffset,maxXOffset);
		/*变道的缓冲值*/
		if(targetOffset>xOffset)
		{
			targetOffsetEnd=targetOffset+0.04f;
		}else
		{
			targetOffsetEnd = targetOffset-0.04f;
		}

		targetOffsetEnd = Mathf.Clamp(targetOffsetEnd,minXOffset,maxXOffset);

		isLockChange=isLock;
		isChangeSpeedUp=true;
	}

	/// <summary>
	/// Updates 左右切道控制
	/// </summary>
	void UpdateOffsetChange()
	{
		if(isChangeXOffset == false)
			return;

		/*控制速度慢时 切道速度*/
		if(speed<45f)
		{
			maxChangeSpeed = maxChangeSpeedDown;
		}else
		{
			maxChangeSpeed = maxChangeSpeedUp;
		}
	
		/*控制切道变速字段 和结束条件*/
		if(isChangeSpeedUp &&  Mathf.Abs(targetOffset - xOffset) <=changeSpeed*Time.deltaTime)
		{
			isChangeSpeedUp=false;

		}else if( !isChangeSpeedUp  && Mathf.Abs(targetOffsetEnd - xOffset) <=changeSpeed*Time.deltaTime)
		{
			isChangeXOffset = false;
			isLockChange = false;
			
			isLefting =false;
			isRighting = false;
			changeSpeed=0;
			return;
		}

		/*控制切道的速度*/
		if(isLockChange)
		{
			changeSpeed= explodeChangeSpeed;
		}else if(isChangeSpeedUp)
		{
			changeSpeed+=changeSpeedAcc;
			changeSpeed = Mathf.Min(changeSpeed,maxChangeSpeed);
		}else
		{
			changeSpeed-=changeSpeedAcc;
			changeSpeed=Mathf.Max(changeSpeed,minChangeSpeed);
		}

		/*左右切道*/
		if(xOffset<targetOffsetEnd)
		{
			xOffset+=changeSpeed*Time.deltaTime;
			isLefting = true;
			isRighting=false;
		}else 
		{
			xOffset-=changeSpeed*Time.deltaTime;
			isRighting = true;
			isLefting=false;
		}
	}

	/// <summary>
	/// 车辆撞飞
	/// </summary>
	public void ExplodeCar()
	{
		isRunning = false;
		rigidBody.useGravity = true;
		rigidBody.mass =100f;
		rigidBody.isKinematic = false;
		float explosionForce = 30.0f;
		rigidBody.AddExplosionForce(explosionForce, selfTrans.position+selfTrans.forward, 2.0f, 3.0f, ForceMode.VelocityChange);
		float forwardForce = 70.0f;
		rigidBody.AddForce(selfTrans.forward * forwardForce, ForceMode.VelocityChange);

		StopCoroutine("IECheckExplodeCarEnd");
		StartCoroutine("IECheckExplodeCarEnd");
	}

	IEnumerator IECheckExplodeCarEnd()
	{
		float time = 2f;
		float orignY = selfTrans.position.y;
		while (time > 0) {
			if (selfTrans.position.y < orignY || selfTrans.position.y - orignY > 40f) {
				gameObject.SetActive (false);
				yield break;
			}
			time -= Time.deltaTime;
			yield return null;
		}
		this.gameObject.SetActive (false);
		rigidBody.useGravity = false;
		rigidBody.isKinematic = true;
	}
	#endregion

	#region  道具控制
	private Vector3 origPos;
	private Vector3 origVec;

	public bool isCarExplode = false;
	/// <summary>
	/// 炸飞效果
	/// </summary>
	public void CarExplode()
	{
		if (gameObject.activeInHierarchy == false)
			return;
		
		speed = 0;
		origPos=selfTrans.position;
		origVec = selfTrans.forward;

		isRunning = false;
		isCarExplode = true;

		StopCoroutine("IECarExplode");
		StartCoroutine("IECarExplode");

		carParticleCon.HideSpark();
		carParticleCon.HideTrace();

		if (propCon.isPlayer)
			CarCameraFollow.Instance.ActiveShake (0.5f, 1.0f);
	}

	IEnumerator IECarExplode()
	{
		float explodeTime = 0.4f;
		float angle = 15;
		if (propCon.isPlayer)
			transform.position += new Vector3 (0, 5, 0);
		else
			transform.position += new Vector3 (0, 8, 0);
		while (explodeTime > 0) {
			transform.Rotate (selfTrans.right, angle, Space.World);
			explodeTime -= Time.deltaTime;
			yield return null;
		}

		selfTrans.position = origPos;
		selfTrans.forward = origVec;

		isRunning = true;
		isCarExplode = false;
	}

	public bool isSliping = false;
	/// <summary>
	/// 赛车打滑效果
	/// </summary>
	public void CarSlip()
	{
		if (gameObject.activeInHierarchy == false)
			return;
		
		speed = 0;
		//origPos = propCon.carTrans.localPosition;
		//origVec = propCon.carTrans.eulerAngles;

		isRunning = false;
		isSliping = true;

		StopCoroutine("IECarSlip");
		StartCoroutine("IECarSlip");

		carParticleCon.HideSpark();
		carParticleCon.HideTrace();
	}
	private IEnumerator IECarSlip()
	{
		float slipTime = 0.2f;
		float angle = 30;
		while(slipTime > 0)
		{
			propCon.carTrans.Rotate (Vector3.up, angle, Space.World);
			slipTime -= Time.deltaTime;
			yield return null;
		}

		//propCon.carTrans.localPosition = origPos;
		propCon.carTrans.localEulerAngles = Vector3.zero;

		isRunning=true;
		isSliping=false;
	}

	public bool isLighting = false;
	/// <summary>
	/// 赛车闪电效果
	/// </summary>
	public void CarLighting(float skillTime)
	{
		if (gameObject.activeInHierarchy == false)
			return;
		
		carParticleCon.PlayParticle (CarParticleType.Lighting, true, 2.0f);
		CarVertigo (skillTime);
	}

	private float vertigoTime;
	/// <summary>
	/// 赛车眩晕
	/// </summary>
	public void CarVertigo(float skillTime)
	{
		if (propCon.isPlayer)
			AudioManger.Instance.PlaySound (AudioManger.SoundName.Stun);
		vertigoTime = skillTime;
		speed = 0;
		isRunning = false;
		isLighting = true;
		carParticleCon.PlayParticle (CarParticleType.Vertigo, true, 1.5f);
		StopCoroutine("IECarVertigo");
		StartCoroutine("IECarVertigo");
	}

	IEnumerator IECarVertigo()
	{
		while(vertigoTime > 0)
		{
			vertigoTime -= Time.deltaTime;
			yield return null;
		}
		isRunning = true;
		isLighting = false;
	}
	#endregion
}
