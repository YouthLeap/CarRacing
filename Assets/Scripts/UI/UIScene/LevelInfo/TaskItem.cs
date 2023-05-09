using UnityEngine;
using System.Collections;

public class TaskItem : MonoBehaviour {

	public EasyFontTextMesh ContentText;
	public GameObject GetImage;

	public void SetContentText(string content)
	{
		ContentText.text = content;
	}

	public void SetGetImage(bool flag)
	{
		GetImage.SetActive (flag);
	}
}
