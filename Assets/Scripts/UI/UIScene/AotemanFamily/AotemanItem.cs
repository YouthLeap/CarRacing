using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AotemanItem : MonoBehaviour {

	public tk2dSprite IconImage;

	public void SetIconImage(string iconName)
	{
		IconImage.SetSprite(iconName);
	}
}
