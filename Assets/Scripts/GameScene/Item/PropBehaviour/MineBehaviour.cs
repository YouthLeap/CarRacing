using UnityEngine;
using System.Collections;

public class MineBehaviour : BehaviourBase {

	public float xOffset;	
	public float moveLen;
	public float upSpeed = 100.0f;
	private CarMove targetCarMove;
	public bool isFlying = false;
	public bool isFlyingUp = false;
	private Transform selfTrans;
	private Vector3 targetPos;

	private float maxHeight;
	private float roadHeight;

	void Awake()
	{
		selfTrans = this.transform;
	}

	public void Launch(float pathLen, float offset, CarMove targetCarMove)
	{
		this.moveLen = pathLen;
		this.xOffset = offset;
		this.targetCarMove = targetCarMove;
		isFlying = true;
		isFlyingUp = true;

		this.maxHeight = targetCarMove.transform.position.y + 50;
	}

	void FixedUpdate () {
		if (isFlying == false)
			return;
		if (GameData.Instance.IsPause)
			return;

		MoveToNextPoint ();
		
		if (isFlyingUp == false && selfTrans.position.y <= this.roadHeight)
			Explode ();
		if (selfTrans.position.y >= this.maxHeight)
			isFlyingUp = false;
	}

	/// <summary>
	/// 赛车的移动控制
	/// </summary>
	void MoveToNextPoint()
	{
		moveLen = targetCarMove.moveLen;
		if (isFlyingUp) {
			targetPos = RoadPathManager.Instance.GetPointByLen (moveLen, xOffset);
			selfTrans.position = new Vector3 (targetPos.x, selfTrans.position.y + upSpeed * Time.deltaTime, targetPos.z);
			if (selfTrans.position.y >= this.maxHeight - 15) {
				for (int i = 0; i < selfTrans.childCount; ++i) {
					selfTrans.GetChild (i).gameObject.SetActive (false);
				}
			}
		} else {
			moveLen += 50;
			targetPos = RoadPathManager.Instance.GetPointByLen (moveLen, xOffset);
			this.roadHeight = targetPos.y;
			selfTrans.position = new Vector3 (targetPos.x, selfTrans.position.y - upSpeed * Time.deltaTime, targetPos.z);
			if (selfTrans.position.y <= this.maxHeight - 15) {
				for (int i = 0; i < selfTrans.childCount; ++i) {
					selfTrans.GetChild (i).gameObject.SetActive (true);
				}
			}
		}
	}

	private float effectRadius = 20;
	NormalCarControl normalCar = null;
	private void Explode ()
	{
		isFlying = false;
		
		SceneParticleController.Instance.PlayParticle ("FX_baozha02_hg", new Vector3 (transform.position.x, this.roadHeight + 1.5f, transform.position.z));

		float carDistance = PlayerCarControl.Instance.carMove.moveLen - selfPropControl.carMove.moveLen;
		if (carDistance >= 0 && carDistance < 45 && Vector3.Distance (transform.position, selfPropControl.transform.position) < 50) {
			CarCameraFollow.Instance.ActiveShake (0.5f, 1.0f);
		}

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, effectRadius);
		for (int i = 0; i < hitColliders.Length; i++) {
			if (!hitColliders[i].CompareTag ("ItemCar"))
				continue;
			propControl = hitColliders [i].transform.GetComponent<PropControl> ();
			normalCar = hitColliders [i].transform.GetComponent<NormalCarControl> ();
			if (propControl != null) {
				propControl.MineHit ();
				if(selfPropControl.isPlayer)
				{
					PropTips.Instance.ShowHit(PropType.Mine);
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