using UnityEngine;
using System.Collections;

public abstract class UIBase : MonoBehaviour {

	public abstract void Init();
	
	public abstract void Show();
	
	public abstract void Hide();
	
	public abstract void Back();
}
