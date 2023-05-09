using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour {
	public static TimeManager Instance;
	private float _realTotaleTime = 0;
	void Awake()
	{
		Instance = this;
	}
	// Use this for initialization
	void Start () {

		_realTotaleTime = 0;
	}
	public float RealTotaleTime
	{
		get{return _realTotaleTime;}
	}
	
	// Update is called once per frame
	void Update () {
		if(GameData.Instance.IsPause)
		{
			return;
		}
		_realTotaleTime += Time.deltaTime;
	}
}
