using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIResAdjust : MonoBehaviour
{
	static float px = -1;
	static float py = -1;

	static void CalculateAdjust()
	{
		var ow = 800f;
		var oh = 480f;

		var w = Screen.width;
		var h = Screen.height;
		var sx = w / ow;
		var sy = h / oh;

		// Spacing on left & right
		px = sx > sy ? ((sx / sy - 1) * ow / 2f) : 0f;

		// Spacing on top & bottom
		py = sy > sx ? ((sy / sx - 1) * oh / 2f) : 0f;
	}
	
	static public Vector3 Adjust(Vector3 v, float xx, float yy)
	{
		if (!Application.isPlaying) return v;
		
		if (xx == 0f && yy == 0f) return v; // not aligned
		if (px == 0f && py == 0f) return v; // no Adjustment needed
		
		if (px == -1) CalculateAdjust ();
		return v + new Vector3(xx * px, yy * py, 0);
	}

	static public float AdjustY(float v)
	{
		if (!Application.isPlaying) return v;
		if (px == -1) CalculateAdjust ();
		return py * v;
	}

	static public float AdjustX(float v)
	{
		if (!Application.isPlaying) return v;
		if (px == -1) CalculateAdjust ();
		return px * v;
	}



	[Range(-1f, 1f)] public float alignX;
	[Range(-1f, 1f)] public float alignY;
	
	void Awake()
	{
		CheckResolution();		
	}
	
	internal void CheckResolution()
	{
		transform.localPosition = Adjust(transform.localPosition, alignX, alignY);
	}
}
