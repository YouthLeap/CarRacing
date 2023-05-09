using UnityEngine;
using System.Collections;

/// <summary>
/// 飞弹 飞行攻击的道具
/// </summary>
public class MissileBehaviour : BehaviourBase {

	public float xOffset;	
	public float moveLen=0; // car position in the path
	public float speed=1f;
	public float acceleration=2f;
	public float maxSpeed=200f;

	public bool isFlying=false;
	private Transform selfTrans;
	private float maxLen=300;
	private float startLen=0;

	private float targetDistance;
	private CarMove targetCarMove;
	private float mOffset;

	void Awake()
	{
		selfTrans = this.transform;
	}

	public void Launch(float pathLen, float offset, CarMove targetCarMove,float mOffset)
	{
		this.startLen = pathLen;
		this.moveLen = pathLen;
		this.xOffset = offset;
		this.mOffset = mOffset;

		this.speed= selfPropControl.carMove.speed;
	
		this.targetCarMove = targetCarMove;

		selfTrans.position = RoadPathManager.Instance.GetPointByLen (pathLen, offset);
		Vector3 ahead = RoadPathManager.Instance.GetPointByLen (pathLen + 1f, offset);
		Vector3 dir = ahead - selfTrans.position;
		selfTrans.forward = dir;

		this.isFlying = true;
	}

	void FixedUpdate () {
		if (GameData.Instance.IsPause)
			return;
	
		if (isFlying == false)
			return;

		if (targetCarMove == null)
			targetDistance = -1;
		else
			targetDistance = Vector3.Distance (selfTrans.position, targetCarMove.transform.position);

		if(targetDistance > 0 && targetDistance < 20)
			MoveToTargetPoint();
		else
			MoveToNextPoint();
	}

	void MoveToTargetPoint()
	{
		selfTrans.position = Vector3.Lerp (selfTrans.position, targetCarMove.transform.position, 40*Time.deltaTime);
	}

	/// <summary>
	/// 赛车的移动控制
	/// </summary>
	void MoveToNextPoint()
	{
		if (targetCarMove != null)
			xOffset = Mathf.Lerp (xOffset, targetCarMove.xOffset, 10 * Time.deltaTime);
		else
			xOffset = Mathf.Lerp (xOffset, mOffset, 8 * Time.deltaTime);

		speed+=acceleration;
		speed= Mathf.Min(speed,maxSpeed);
		moveLen+=speed * Time.deltaTime;
		selfTrans.position = RoadPathManager.Instance.GetPointByLen(moveLen,xOffset);

		Vector3 ahead= RoadPathManager.Instance.GetPointByLen(moveLen + 1f, xOffset);
		Vector3 dir = ahead - selfTrans.position;
		selfTrans.forward = Vector3.Lerp(selfTrans.forward, dir,10*Time.deltaTime);

		//recycle
		if(moveLen-startLen >maxLen)
		{
			Recycle ();
		}
			
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag ("ItemCar")) {
			CarMove carMove = col.transform.GetComponent<CarMove> ();
			normalCar = col.transform.GetComponent<NormalCarControl> ();
			if (carMove != null && carMove == targetCarMove) {
				Explode ();
			}
			if (targetCarMove == null && normalCar != null) {
				Explode ();
			}
		}
	}

	private float effectRadius = 5;
	NormalCarControl normalCar = null;
	private void Explode ()
	{
		isFlying = false;

		SceneParticleController.Instance.PlayParticle ("FX_daodan02_hg",transform.position + Vector3.up * 0.3f);

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, effectRadius);
		for (int i = 0; i < hitColliders.Length; i++) {
			if (!hitColliders[i].CompareTag ("ItemCar"))
				continue;
			propControl = hitColliders [i].transform.GetComponent<PropControl> ();
			normalCar = hitColliders [i].transform.GetComponent<NormalCarControl> ();
			if (propControl != null && propControl != selfPropControl) {
				propControl.MissileHit ();
				if(selfPropControl.isPlayer)
				{
					PropTips.Instance.ShowHit(PropType.Missile);
				}
			} else if (normalCar != null) {
				normalCar.carMove.ExplodeCar ();
				if(selfPropControl.isPlayer)
				{
					normalCar.GetComponent<NormalCarItem>().AddCoinByID();
				}
			}
		}

		if(selfPropControl.isPlayer)
		{
			AudioManger.Instance.PlaySound(AudioManger.SoundName.MissileExplode);
		}

		Recycle ();
	}

}