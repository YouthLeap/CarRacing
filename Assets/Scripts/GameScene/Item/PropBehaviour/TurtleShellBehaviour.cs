using UnityEngine;
using System.Collections;
using DG.Tweening;

public class TurtleShellBehaviour : BehaviourBase {

	public enum ShellType{Green,Red};
	public ShellType shellType;

	public float xOffset;	
	public float moveLen=0; // car position in the path
	public float speed=1f;

	public Transform shellTrans;

	public bool isFlying = false;
	private Transform selfTrans;
	private float maxLen=400;
	private float startLen=0;

	private float maxSpeed=0f;
	private float acceleration=2f;

	void Awake()
	{
		selfTrans = this.transform;
	}

	public void Launch(float pathLen, float offset, float speed=300f)
	{
		this.startLen = pathLen;
		this.moveLen = pathLen;
		this.xOffset = offset;
		this.speed = selfPropControl.carMove.speed;
		this.maxSpeed=speed;

		selfTrans.position = RoadPathManager.Instance.GetPointByLen (pathLen, offset);
		Vector3 ahead = RoadPathManager.Instance.GetPointByLen (pathLen + 1f, offset);
		Vector3 dir = ahead - selfTrans.position;
		selfTrans.forward = dir;

		this.isFlying = true;
		shellTrans.DOKill();
		shellTrans.DOLocalRotate(new Vector3(0,180,0),0.5f).SetLoops(-1,LoopType.Restart);
	}

	void FixedUpdate () {

		if (isFlying == false)
			return;

		MoveToNextPoint();
	}

	/// <summary>
	/// 赛车的移动控制
	/// </summary>
	void MoveToNextPoint()
	{
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
			base.Recycle ();
		}
	}

	NormalCarControl normalCar = null;
	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag ("ItemCar")) {
			propControl = col.transform.GetComponent<PropControl> ();
			if (propControl != null && propControl != selfPropControl) {
				SceneParticleController.Instance.PlayParticle ("explosion", transform.position + Vector3.up * 3);
				propControl.TurtleShellHit ();
				isFlying = false;
				//recycle
				base.Recycle ();

				if(propControl.isPlayer)
				{
					if(shellType == ShellType.Green)
					{
						PropTips.Instance.ShowBeHit(PropType.GreenTurtleShell);
					}else
					{
						PropTips.Instance.ShowBeHit(PropType.RedTurtleShell);
					}
				}

				if(selfPropControl.isPlayer)
				{
					if(shellType == ShellType.Green)
					{
						PropTips.Instance.ShowHit(PropType.GreenTurtleShell);
					}else
					{
						PropTips.Instance.ShowHit(PropType.RedTurtleShell);
					}
				}

			} else if ((normalCar = col.transform.GetComponent<NormalCarControl> ()) != null) {
				SceneParticleController.Instance.PlayParticle ("explosion", transform.position + Vector3.up * 3);
				normalCar.carMove.ExplodeCar ();
				isFlying = false;
				if(selfPropControl.isPlayer)
				{
					normalCar.GetComponent<NormalCarItem>().AddCoinByID();
				}

				base.Recycle ();
			}
		}
	}
}
