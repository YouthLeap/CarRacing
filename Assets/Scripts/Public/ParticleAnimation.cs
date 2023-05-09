using UnityEngine;
using System.Collections;

public class ParticleAnimation : MonoBehaviour {
	public ParticleSystem ParticleEffect;
	public float scrollXSpeed = 0f;
	public float scrollYSpeed = 0f;
	public bool isLoop = true;
	private float offsetX,offsetY;

	void LateUpdate () {

        offsetX += scrollXSpeed;
		offsetY += scrollYSpeed;
		if(!isLoop && Mathf.Abs(offsetX) >= 1)
			return;
		GetComponent<Renderer>().sharedMaterial.mainTextureOffset = new Vector2(offsetX, offsetY);

	}

	void OnDisable()
	{
		GetComponent<Renderer>().sharedMaterial.mainTextureOffset = Vector2.zero;
	}
}
