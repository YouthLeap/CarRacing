using UnityEngine;
using System.Collections;

public class MechaAnimatorManager : MonoBehaviour {
	private Animator mechaAnim;
	private AnimatorStateInfo animState;
	
	public void Init()
	{
		mechaAnim = GetComponent<Animator>();
		MechaAnimStateCallBack[] states = mechaAnim.GetBehaviours<MechaAnimStateCallBack>();
		for(int i = 0; i < states.Length; i ++)
		{
			states[i].animControllor = this;
		}
	}
#if false

	// Use this for initialization
	void Start () {
		Init ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.O))
		{
			Open();
		}
		else if(Input.GetKeyDown(KeyCode.DownArrow))
		{

		}
		else if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			LeftMove();
		}
		else if(Input.GetKeyUp(KeyCode.LeftArrow))
		{
			LeftMoveBack();
		}
		else if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			RightMove();
		}else if(Input.GetKeyUp(KeyCode.RightArrow))
		{
			RightMoveBack();
		}else if(Input.GetKeyDown(KeyCode.S))
		{
			Show();
		}
		else if(Input.GetKeyDown(KeyCode.F))
		{
			Fail();
		}
		else if(Input.GetKeyDown(KeyCode.I))
		{
			Idle();
		}
	
	}
#endif
	public void Idle()
	{
		mechaAnim.SetTrigger ("IdleTrigger");
	}
	public void Show()
	{
		mechaAnim.SetTrigger ("ShowTrigger");
	}
	public void Open()
	{
		mechaAnim.SetTrigger ("OpenTrigger");
	}
	public void LeftMove()
	{
		mechaAnim.ResetTrigger ("LeftMoveBackTrigger");
		mechaAnim.SetTrigger ("LeftMoveTrigger");
	}
	public void LeftMoveBack()
	{
		mechaAnim.SetTrigger ("LeftMoveBackTrigger");
	}
	public void RightMove()
	{
		mechaAnim.ResetTrigger ("RightMoveBackTrigger");
		mechaAnim.SetTrigger ("RightMoveTrigger");
	}
	public void RightMoveBack()
	{
		mechaAnim.SetTrigger ("RightMoveBackTrigger");
	}
	public void Attack()
	{
		mechaAnim.SetTrigger ("AttackTrigger");
	}
	public void BeAttack()
	{
		mechaAnim.SetTrigger ("BeAttackTrigger");
	}
	public void Fail()
	{
		mechaAnim.SetTrigger ("FailTrigger");
	}
	#region 动作回调函数
	public void MotionEnterHandler(int shortNameHash)
	{

	}
	public void MotionExitHandler(int shortNameHash)
	{
//		if(shortNameHash == AnimatorNameDefine.ChuChangkState)
//		{
//			PlayerParticleController.Instance.PlayGroundSmashParticle();
//		}
	}
	#endregion
}
