using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

[ExecuteInEditMode]
public class MeshRenderSettingTool : MonoBehaviour {
#if UNITY_EDITOR
	public bool apply = false;
	
	void Update () {
		if(apply)
		{
			apply = false;
			SettingMeshRender(transform);
		}	
	}
	
	void SettingMeshRender(Transform parent)
	{
		for(int i = 0; i < parent.childCount; i++)
		{
			MeshRenderer mr = parent.GetChild(i).GetComponent<MeshRenderer>();
			if(mr != null)
			{
			mr.shadowCastingMode = ShadowCastingMode.Off;
			mr.receiveShadows = false;
			mr.useLightProbes = false;
			mr.reflectionProbeUsage = ReflectionProbeUsage.Off;
			}
			SettingMeshRender(parent.GetChild(i));
		}
	}
#endif
}
