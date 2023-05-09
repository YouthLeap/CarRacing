using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DropCoinItem : MonoBehaviour {

	public EasyFontTextMesh coutText;

	public void Show(Vector3 offsetPos)
	{
		coutText.gameObject.SetActive (false);
		gameObject.SetActive (true);
		//coutText.text ="+" +count.ToString ();
		Sequence seq = DOTween.Sequence();
		seq.Append ( transform.DOMove (transform.position + offsetPos, 0.4f).SetEase (Ease.OutExpo) ); 
		seq.Append (transform.DOMove(new Vector3(186,213,0),0.5f));
		seq.OnComplete (Recycle);
	}

	void Recycle()
	{
		gameObject.SetActive (false);
		DropItemManage.Instance.Recycle (this.transform);
	}

	public void ShowOneAndMoveToBar()
	{
		coutText.gameObject.SetActive (false);
		Sequence seq = DOTween.Sequence();
		seq.Append ( transform.DOMove (transform.position + new Vector3 (40f+Random.value*40, 40f+Random.value*40f, 0), 0.6f).SetEase (Ease.OutExpo) ); 
		seq.Append (transform.DOMove(new Vector3(186,213,0),0.6f));
		seq.OnComplete (Recycle);

	}

	public void ShowExplode(float range=4f)
	{
		coutText.gameObject.SetActive (false);
		Vector3 newPos = transform.position + new Vector3 (Random.Range (-range, range), Random.Range (-range, range), 0);
		Sequence seq = DOTween.Sequence();
		seq.Append ( transform.DOMove (newPos, 0.3f).SetEase (Ease.OutExpo) ); 
		seq.Append (transform.DOMove(new Vector3(258,218,0),0.6f).SetEase (Ease.OutExpo));
		seq.OnComplete (Recycle);

	}
}
