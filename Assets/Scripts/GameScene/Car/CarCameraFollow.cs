using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// 跟随镜头
/// </summary>
public class CarCameraFollow : MonoBehaviour {

	public static CarCameraFollow Instance= null;


	public float angle = 20; 
	public float distance = -10f; 
	public float height = 8.5f; 

	public float followSpeed = 20;

	public Vector3 rotateCamera;
	public GameObject SpeedUpEffectGO;
	public GameObject LightingEffectGO;

	private Transform target; 
	private Vector3 targetVec;
	private Vector3 targetPos;
	private Quaternion playerQua;
	private PropControl propCon;
	private CarMove carMove;
	private Vector3 posCamera;

	private bool isStartRun = false;

	private Transform selfTrans;
	void Awake()
	{
		Instance= this;
		selfTrans= this.transform;
	}

	// Use this for initialization
	void Start () {
	   
		target = PlayerCarControl.Instance.transform;

		propCon = PlayerCarControl.Instance.propCon;
		carMove = PlayerCarControl.Instance.carMove;

		isStartRun = false;

	}

	void FixedUpdate()
	{
		if (isStartAnim) {
			turnAngle += 50 * Time.deltaTime;

			if (turnAngle > 180)
				turnAngle = 180;

			curAngle = startAngle + turnAngle;

			percent = turnAngle / 180;
			if (percent > 1)
				percent = 1;
			cHeight = sHeight - xHeight * percent;
			cAngle = sAngle - xAngle * percent;

			transform.position = new Vector3 (GetPositionX (curAngle), cHeight, GetPositionZ (curAngle));
			transform.eulerAngles = new Vector3 (cAngle, 270 - curAngle, 0);
			if (turnAngle >= 180) {
				isStartAnim = false;
				isStartRun = true;
				GameUIManager.Instance.ShowModule (UISceneModuleType.GameSkill);
			}
			return;
		}

		if (isEndAnim) {
			return;
		}

		if (isStartRun == false)
			return;
		
		if(carMove.isSliping || carMove.isLighting || carMove.isCarExplode)
		{
			return;
		}
		if(target != null){

			if(carMove.curCenterMoveDir == Vector3.zero)
			{
				SetupStartPos();
				return;
			}

			targetVec = carMove.curCenterMoveDir.normalized;
			targetVec.y=0;
			targetPos = target.position + targetVec*distance;
			targetPos.y+=height;

			posCamera = Vector3.Lerp (selfTrans.position, targetPos, followSpeed * Time.smoothDeltaTime);

			//设定位置.
			selfTrans.position = posCamera;

			if (shakeFlag) {
				posCamera += selfTrans.right * Random.Range (-shakeValue, shakeValue);
				selfTrans.position = posCamera;
			}
						
			//rotate
			playerQua=   Quaternion.LookRotation(carMove.curCenterMoveDir);
			rotateCamera = playerQua.eulerAngles;
			rotateCamera.x=angle;
			transform.rotation=Quaternion.Lerp(transform.rotation,Quaternion.Euler(rotateCamera), 5*Time.smoothDeltaTime);
		}
	}

	public void SetSpeedUp(bool isSpeedUp)
	{
		if (isSpeedUp)
			DOTween.To (() => this.followSpeed, x => this.followSpeed = x, 10, 0.2f);
		else
			DOTween.To (() => this.followSpeed, x => this.followSpeed = x, 20, 1.0f);
	}

	public void SetChangeBig(bool isChangeBig)
	{
		if (isChangeBig) {
			DOTween.To (() => this.angle, x => this.angle = x, 15, 0.3f);
			DOTween.To (() => this.distance, x => this.distance = x, -15.1f, 0.3f);
			DOTween.To (() => this.height, x => this.height = x, 10.55f, 0.3f);
		} else {
			DOTween.To (() => this.angle, x => this.angle = x, 10, 0.3f);
			DOTween.To (() => this.distance, x => this.distance = x, -12.1f, 0.3f);
			DOTween.To (() => this.height, x => this.height = x, 6.55f, 0.3f);
		}
	}

	private bool shakeFlag = false;
	private float shakeValue;
	private float shakeTime;
	//窗口振动.
	public void ActiveShake(float time, float value) {
		shakeTime = time;
		shakeValue = value;
		StopCoroutine ("ShakeCamera");
		StartCoroutine ("ShakeCamera");
	}

	IEnumerator ShakeCamera() {
		shakeFlag = true;
		while (shakeTime > 0) {
			shakeTime -= Time.smoothDeltaTime;	
			yield return null;
		}
		shakeFlag = false;
	}

	/// <summary>
	/// 设定开始位置和旋转方向
	/// </summary>
	public void SetupStartPos()
	{
		targetVec = carMove.transform.forward;
		
		//Debug.Log (targetVec);
		targetVec.y=0;
		targetPos = target.position + targetVec*distance;
		targetPos.y+=height;
		//设定位置.				
		transform.position = targetPos;
		
		//rotate
		playerQua=   Quaternion.LookRotation(targetVec);
		rotateCamera = playerQua.eulerAngles;
		rotateCamera.x=angle;
		transform.rotation=Quaternion.Euler(rotateCamera);

	}

	public void SetSpeedUpEffect(bool isShow)
	{
		SpeedUpEffectGO.SetActive(isShow);
	}

	public void SetLightingEffect(bool isShow)
	{
		LightingEffectGO.SetActive(isShow);
	}

	private bool isStartAnim = false;
	public void ShowStartAnim()
	{
		r = Mathf.Abs (distance);
		center = new Vector3 (target.position.x, target.position.y + height, target.position.z);

		if (Mathf.Abs ((target.eulerAngles.y % 180 - 90)) > 80)
			startAngle = target.eulerAngles.y + 90;
		else
			startAngle = target.eulerAngles.y - 90;

		//取整
		startAngle = (int)(startAngle / 10) * 10;

		endAngle = startAngle + 180;
		curAngle = startAngle;
		turnAngle = 0;

		sHeight = 10;
		sAngle = 20;
		xHeight = sHeight - center.y;
		xAngle = sAngle - angle;
		percent = 0;

		//transform.position = new Vector3 (GetPositionX (curAngle), target.position.y + height, GetPositionZ (curAngle));
		//transform.eulerAngles = new Vector3 (angle, 270 - curAngle, 0);
		transform.position = new Vector3 (GetPositionX (curAngle), sHeight, GetPositionZ (curAngle));
		transform.eulerAngles = new Vector3 (sAngle, 270 - curAngle, 0);

		/*
		endMovePos = new Vector3 (GetPositionX (curAngle), sHeight, GetPositionZ (curAngle));
		dirVec = (endMovePos - target.position).normalized;
		dirVec.y = 0;
		startMovePos = endMovePos + dirVec * 15;
		centerAngle = 12;
		this.pAngle = centerAngle;
		transform.position = startMovePos;
		transform.eulerAngles = new Vector3 (centerAngle, 270 - curAngle, 0);
		*/

		StopCoroutine ("IEStartAnim");
		StartCoroutine ("IEStartAnim");
	}

	private IEnumerator IEStartAnim()
	{
		yield return new WaitForSeconds (0.2f);
		//transform.DOMove (endMovePos, 2.0f).SetEase (Ease.Linear);
		//DOTween.To (() => this.pAngle, x => this.pAngle = x, sAngle, 2.0f).OnUpdate (ChangeEulerAngles).SetEase (Ease.Linear);
		//yield return new WaitForSeconds (2.0f);
		isStartAnim = true;
	}

	private void ChangeEulerAngles()
	{
		transform.eulerAngles = new Vector3 (this.pAngle, 270 - curAngle, 0);
	}

	private float r;//半径
	private Vector3 center;//圆心
	private float turnAngle;
	private float startAngle, endAngle, curAngle;
	private float sHeight, sAngle, xHeight, xAngle, cHeight, cAngle, percent;
	private Vector3 startMovePos, endMovePos, dirVec;
	private float centerAngle;
	[HideInInspector]
	public float pAngle;
	private float GetPositionX(float angle)
	{
		return center.x + r * Mathf.Cos (angle * 3.14f / 180);
	}

	private float GetPositionZ(float angle)
	{
		return center.z + r * Mathf.Sin (angle * 3.14f / 180);
	}

	private bool isEndAnim = false;
	public void ShowEndAnim()
	{
		
	}
}