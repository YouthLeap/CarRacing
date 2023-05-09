using UnityEngine;
using System.Collections;

public class AnimatorManager : MonoBehaviour {


	private Animator anim;
	private AnimatorStateInfo animState;
	
	private Vector3 moveDelta = Vector3.zero;
	
	public void Init()
	{
		anim = GetComponent<Animator>();
		AnimatorStateCallBack[] states = anim.GetBehaviours<AnimatorStateCallBack>();
		for(int i = 0; i < states.Length; i ++)
		{
			states[i].animatorController = this;
		}
	}
	
#if false
	
	public bool testMode = true;
	public float graity = 20, jumpValue = 7;
	private bool isEnlarge = false;
	public bool isJumpOnce = false, isJumpTwice = false;

	void Awake()
	{
		anim = GetComponent<Animator>();
		Init();
	}
	bool speeduping = false;
	float speedtime = 2;
	IEnumerator IESpeedUp()
	{
		speedtime = 10;
		speeduping = true;
		SpeedUp();
		while(speedtime > 0)
		{
			speedtime -= Time.deltaTime;
			yield return null;
		}
		SpeedUpBack ();
		speeduping = false;
	}
	void Update () {
		if(!testMode)
			return;
		
		animState = anim.GetCurrentAnimatorStateInfo(0);
		
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			LeftMove();
		} else if (Input.GetKeyUp (KeyCode.LeftArrow)) {
			LeftMoveBack();
		}
		else if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			RightMove();
		}
		else if(Input.GetKeyUp (KeyCode.RightArrow)){
			RightMoveBack();

		}else if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			StartCoroutine("IESpeedUp");

		}
		else if(Input.GetKeyDown(KeyCode.P))
		{
			PettyAction();
		}else if(Input.GetKeyDown(KeyCode.W))
		{
			Win();
		}else if(Input.GetKeyDown(KeyCode.F))
		{
			Fail();
		}else if(Input.GetKeyDown(KeyCode.C))
		{
			Crash();
		}
		else if(Input.GetKeyDown(KeyCode.S))
		{
			Show();
		}
	}

#endif	

	#region 动作
	public void Setting()
	{
		anim.SetTrigger("SittingTrigger");
	}
	int showCount = 1;
	public void Show()
	{
		if (!anim.gameObject.activeInHierarchy) return;

		if (showCount == 1)
			anim.SetTrigger ("ShowTrigger1");
		else
			anim.SetTrigger ("ShowTrigger2");
		
		showCount = ~showCount;
	}
	public void Show1()
	{
		if (!anim.gameObject.activeInHierarchy) return;
		anim.SetTrigger ("ShowTrigger1");
	}
	public void Show2()
	{
		if (!anim.gameObject.activeInHierarchy) return;
		anim.SetTrigger ("ShowTrigger2");
	}
	public void LeftMove()
	{
		if (!anim.gameObject.activeInHierarchy) return;
		anim.ResetTrigger ("LeftMoveBackTrigger");
		anim.SetTrigger("LeftMoveTrigger");
	}
	public void LeftMoveBack()
	{
		if (!anim.gameObject.activeInHierarchy) return;
		anim.SetTrigger("LeftMoveBackTrigger");
	}
	public void RightMove()
	{
		anim.ResetTrigger ("RightMoveBackTrigger");
		anim.SetTrigger ("RightMoveTrigger");
	}
	public void RightMoveBack()
	{

		anim.SetTrigger ("RightMoveBackTrigger");
	}
	public void SpeedUp()
	{
		if (!anim.gameObject.activeInHierarchy) return;
		anim.ResetTrigger ("SpeedUpBackTrigger");
		anim.SetTrigger ("SpeedUpTrigger");
	}
	public void SpeedUpBack()
	{
		if (!anim.gameObject.activeInHierarchy) return;
		anim.SetTrigger ("SpeedUpBackTrigger");
	}
	public void PettyAction()
	{
		if (!anim.gameObject.activeInHierarchy) return;
		anim.SetTrigger ("PettyActionTrigger");
	}
	public void Crash()
	{
		if (!anim.gameObject.activeInHierarchy) return;
		anim.SetTrigger ("CrashTrigger");
	}
	public void Win()
	{
		if (!anim.gameObject.activeInHierarchy) return;
		anim.SetTrigger ("WinTrigger");
	}
	public void Fail()
	{
		if (!anim.gameObject.activeInHierarchy) return;
		anim.SetTrigger ("FailTrigger");
	}
	AttackState attackState = AttackState.NoneAttack;
	public void Attack()
	{
		CancelInvoke("ResetAttackState");
		if (!anim.gameObject.activeInHierarchy) return;

		if(attackState == AttackState.NoneAttack)
		{
			anim.SetTrigger("Attack1Trigger");
		}else if(attackState == AttackState.Attack1)
		{
			anim.SetTrigger("Attack2Trigger");
		}else if(attackState == AttackState.Attack2)
		{
			anim.SetTrigger("Attack3Trigger");
		}
	}
	
	void ResetAttackState()
	{
		attackState = AttackState.NoneAttack;
	}

	#endregion
	
	#region 动作回调函数
	
	/// <summary>
	/// 动作开始的回调函数.
	/// </summary>
	/// <param name="shortNameHash">Short name hash.</param>
	public void MotionEnterHandler(int shortNameHash)
	{
		if(shortNameHash == AnimatorNameDefine.Attack1State)
		{
//			attackState = AttackState.Attack1;
//			PlayerController.Instance.isAttacking = true;
//			PlayerController.Instance.PlayAttack1Particle();
//			AudioManger.Instance.PlaySound(AudioManger.SoundName.FirstAttack);
		}else if(shortNameHash == AnimatorNameDefine.Attack2State)
		{
//			attackState = AttackState.Attack2;
//			PlayerController.Instance.isAttacking = true;
//			PlayerController.Instance.PlayAttack2Particle();
//			AudioManger.Instance.PlaySound(AudioManger.SoundName.SeconedAttack);
		}else if(shortNameHash == AnimatorNameDefine.Attack3State)
		{
//			attackState = AttackState.Attack3;
//			PlayerController.Instance.isAttacking = true;
//			PlayerController.Instance.PlayAttack3Particle();
//			AudioManger.Instance.PlaySound(AudioManger.SoundName.ThirdAttack);
//			AudioManger.Instance.PlaySound(AudioManger.SoundName.Exert);
		}else
		{
//			PlayerController.Instance.isAttacking = false;
//			if(attackState == AttackState.Attack3)
//				ResetAttackState();
//			else if(attackState != AttackState.NoneAttack)
//				Invoke("ResetAttackState", 0.5f);
		}
	}
	
	/// <summary>
	/// 动作退出的回调函数.
	/// </summary>
	/// <param name="shortNameHash">Short name hash.</param>
	public void MotionExitHandler(int shortNameHash)
	{
		if (shortNameHash != AnimatorNameDefine.SpeedUpBackState) {
//			if (PlayerController.Instance.InvincibleSpeedUp) {
//				SpeedUp ();
//			}
		} 
//		if(shortNameHash == AnimatorNameDefine.Attack1State || shortNameHash == AnimatorNameDefine.Attack2State || shortNameHash == AnimatorNameDefine.Attack3State)
//		{
//			PlayerController.Instance.isAttacking = false;
//			if(attackState == AttackState.Attack3)
//				ResetAttackState();
//			else if(attackState != AttackState.NoneAttack)
//				Invoke("ResetAttackState", 0.5f);
//		}
	}
	
	#endregion
	
	public void SetSpeed(float speed)
	{
		anim.speed = speed;
	}
	
	IEnumerator ResetFloatValue(string key, float value, float time)
	{
		while(time > 0)
		{
			time -= Time.deltaTime;
			yield return 0;
			
		}
		
		anim.SetFloat(key, value);
	}
	
	IEnumerator ResetBoolValue(string key, bool value, float time)
	{
		while(time > 0)
		{
			time -= Time.deltaTime;
			yield return 0;
			
		}
		
		anim.SetBool(key, value);
	}
	
	#region 攻击状态
	
	enum AttackState
	{
		NoneAttack,
		Attack1,
		Attack2,
		Attack3,
	}
	
	#endregion
	
}
