using UnityEngine;
using System.Collections;

/// <summary>
/// 人物动作状态变化的回调类.
/// </summary>
public class AnimatorStateCallBack : StateMachineBehaviour {

	[HideInInspector]public AnimatorManager animatorController;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(animatorController != null)
			animatorController.MotionEnterHandler(stateInfo.shortNameHash);
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(animatorController != null)
			animatorController.MotionExitHandler(stateInfo.shortNameHash);
	}
	
}
