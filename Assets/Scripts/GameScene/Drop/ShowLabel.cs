using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ShowLabel : MonoBehaviour {

	public EasyFontTextMesh coutText;
	
	public void Show(string text)
	{
		gameObject.SetActive (true);
		coutText.text =text;

		transform.position = new Vector3 (-600f,0,0);

		Sequence mySequence = DOTween.Sequence();
		mySequence.Append ( transform.DOMove (Vector3.zero, 0.4f).SetEase (Ease.OutBounce));
		mySequence.Append( transform.DOMove(new Vector3(600f,0f,0f),0.4f).SetEase(Ease.InBounce).SetDelay(0.8f) );
		mySequence.OnComplete (Recycle);
	}
	
	void Recycle()
	{
		gameObject.SetActive (false);
		DropItemManage.Instance.Recycle (this.transform);
	}
}
