using UnityEngine;
using System.Collections;

public class ActorCameraController2 : MonoBehaviour {

	public static ActorCameraController2 Instance;
	
	private Camera actorCamera;
	
	void Awake ()
	{
		Instance = this;
	}
	
	void Start ()
	{
		actorCamera = transform.GetComponent<Camera> ();
	}
	
	public void SetCameraDepth (int depth)
	{
		actorCamera.depth = depth;
	}
}
