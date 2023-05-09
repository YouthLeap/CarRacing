using UnityEngine;
using System.Collections;

public class MechaAnimStateCallBack : StateMachineBehaviour {
	[HideInInspector]public MechaAnimatorManager animControllor;
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(animControllor != null)
			animControllor.MotionEnterHandler(stateInfo.shortNameHash);
	}
	
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(animControllor != null)
			animControllor.MotionExitHandler(stateInfo.shortNameHash);
	}
}
