using UnityEngine;
using System.Collections;
using DG.Tweening;

public class LightingBehaviour : BehaviourBase {
	public void Show()
	{
		transform.DOLocalMoveY (15, 0.5f).SetEase (Ease.Linear).OnComplete (Recycle);
	}
}