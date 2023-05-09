using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ParticleLayer : MonoBehaviour {

	public int OrderInLayer = 0;
	public bool ApplyNow = false;
	public bool ApplyAllChild = false;

	void Awake()
	{
#if UNITY_EDITOR
		if(Application.isPlaying)
#endif
		ApplySortOrder(transform);
	}

#if UNITY_EDITOR
	void Update()
	{
		if(ApplyNow)
		{
			ApplyNow = false;
			ApplySortOrder(transform);
		}

		if (ApplyAllChild) {
			ApplyAllChild = false;
			ApplyAllChildSortOrder (transform);
		}
	}
#endif

	private void ApplySortOrder(Transform tran)
	{
		if(tran.GetComponent<Renderer>() != null)
		{
			tran.GetComponent<Renderer>().sortingOrder = OrderInLayer;
		}
	}

	private void ApplyAllChildSortOrder(Transform tran)
	{
		int count = tran.childCount;
		for (int i = 0; i < count; i++) {
			ApplySortOrder (tran.GetChild(i));
			ApplyAllChildSortOrder (tran.GetChild(i));
		}
	}
}
