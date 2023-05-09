using UnityEngine;
using System.Collections;

public class ActorCameraController1 : MonoBehaviour {

	public static ActorCameraController1 Instance;

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