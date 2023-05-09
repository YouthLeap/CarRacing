using UnityEngine;
using System.Collections;

/// <summary>
/// 低端手机的粒子特效控制
/// </summary>
public class QualityParticleControl : MonoBehaviour {

	// Use this for initialization
	void OnEnable() {

		/*低端手机排除掉特效*/
		if (QualitySetting.deviceQualityLevel != DeviceQualityLevel.Low) {
			return;
		}

		ParticleSystem particle = GetComponent<ParticleSystem> ();
		if (particle != null) {
			particle.gameObject.SetActive(false);
		}

		ParticleSystem[] pars = GetComponentsInChildren<ParticleSystem> ();
		for (int i=0; i<pars.Length; ++i) {
			pars[i].gameObject.SetActive(false);
		}
	
	}

}
