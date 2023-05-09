using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
[AddComponentMenu ("ImageEffect/RadialBlur")]
public class CameraEffect : MonoBehaviour {
	#region Variables
	public Shader RadialBlurShader = null;
	private Material RadialBlurMaterial = null;
	
	[Range(0.0f, 1.0f)]
	public float SampleDist = 0.17f;

	[Range(1.0f, 5.0f)]
	public float SampleStrength = 1.3f;
	public bool isUse=false;
	[Range(0f,1f)]
	public float centerX=0.5f;
	[Range(0f,1f)]
	public float centerY=0.5f;

	public static CameraEffect Instance=null;


	#endregion
	
	public void StartEffect()
	{
		isUse=true;
	}
	public void StopEffect()
	{
		isUse=false;
	}

	void Awake()
	{
		Instance=this;
	}
   void Start () {
		FindShaders ();
		CheckSupport ();
		CreateMaterials ();
	}

	void FindShaders () {
		if (!RadialBlurShader) {
			RadialBlurShader = Shader.Find("PengLu/ImageEffect/Unlit/RadialBlur");
		}
	}

	void CreateMaterials() {
		if(!RadialBlurMaterial){
			RadialBlurMaterial = new Material(RadialBlurShader);
			RadialBlurMaterial.hideFlags = HideFlags.HideAndDontSave;	
		}
	}

	bool Supported(){
		return (SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures && RadialBlurShader.isSupported);
		// return true;
	}


	bool CheckSupport() {
		if(!Supported()) {
			enabled = false;
			return false;
		}
		// rtFormat = SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.RGB565) ? RenderTextureFormat.RGB565 : RenderTextureFormat.Default;
		return true;
	}


	void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
	{	
		#if UNITY_EDITOR
			FindShaders ();
			CheckSupport ();
			CreateMaterials ();	
		#endif

		if(isUse &&  SampleDist != 0 && SampleStrength != 0){

			int rtW = sourceTexture.width/8;
	        int rtH = sourceTexture.height/8;


	        RadialBlurMaterial.SetFloat ("_SampleDist", SampleDist);
	        RadialBlurMaterial.SetFloat ("_SampleStrength", SampleStrength);	
			RadialBlurMaterial.SetFloat ("_CenterX", centerX);	
			RadialBlurMaterial.SetFloat ("_CenterY", centerY);	


	        RenderTexture rtTempA = RenderTexture.GetTemporary (rtW, rtH, 0,RenderTextureFormat.Default);
            rtTempA.filterMode = FilterMode.Bilinear;

            Graphics.Blit (sourceTexture, rtTempA);

            RenderTexture rtTempB = RenderTexture.GetTemporary (rtW, rtH, 0,RenderTextureFormat.Default);
            rtTempB.filterMode = FilterMode.Bilinear;
            // RadialBlurMaterial.SetTexture ("_MainTex", rtTempA);
            Graphics.Blit (rtTempA, rtTempB, RadialBlurMaterial,0);

            RadialBlurMaterial.SetTexture ("_BlurTex", rtTempB);
       		Graphics.Blit (sourceTexture, destTexture, RadialBlurMaterial,1);

            RenderTexture.ReleaseTemporary(rtTempA);
            RenderTexture.ReleaseTemporary(rtTempB);
 
		}

		else{
			Graphics.Blit(sourceTexture, destTexture);
			
		}
		
		
	}
	
	 public void OnDisable () {
        if (RadialBlurMaterial)
            DestroyImmediate (RadialBlurMaterial);
            // RadialBlurMaterial = null;
    }
}
