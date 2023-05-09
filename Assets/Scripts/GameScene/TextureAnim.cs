using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class TextureAnim : MonoBehaviour {
	public float dt = 0.1f;
	public Vector2 shift = new Vector2(0,1);
	public bool smoon = false;
	public int  materialIndex = 0;
	private Material material;

	// Use this for initialization
	void Start () {
		material = GetComponent<MeshRenderer> ().sharedMaterials[materialIndex];
	}
	
	// Update is called once per frame
	void Update () {
		float time = TimeManager.Instance.RealTotaleTime;
		Vector2 offset = Vector2.zero, scale = material.mainTextureScale;
		if (smoon) {
			offset = shift * (time / dt);
		} else {
			offset = shift * (Mathf.FloorToInt(time / dt));
		}
		material.mainTextureOffset = new Vector2 (scale.x > 0 ? offset.x - Mathf.Floor(offset.x) : 0,scale.y > 0 ? offset.y - Mathf.Floor(offset.y) : 0);
	}
}
