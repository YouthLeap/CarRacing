using UnityEngine;  
using System.Collections;  

[ExecuteInEditMode]
public class CameraMotionBlurEffects : MonoBehaviour  
{  
    public static CameraMotionBlurEffects Instance;

    public bool EnableEffect = false, isTurnOn = false;

    public Shader CurShader;//着色器实例  
    private Vector4 ScreenResolution;//屏幕分辨率  
    private Material CurMaterial;//当前的材质  

    [Range(1, 50)]  
    public float IterationNumber = 15;  
    [Range(-0.1f, 0.1f)]  
    public float Intensity = 0.125f;  
    [Range(-2f, 2f)]  
    public float OffsetX = 0.5f;  
    [Range(-2f, 2f)]  
    public float OffsetY = 0.5f;  

    //临时记录变量
    public float IntensityChangeValue = 0, IntensityTemp = 0;  

    private float IntensityChangeDelta = 0.001f;

    Material material  
    {  
        get  
        {  
            if (CurMaterial == null)  
            {  
                CurMaterial = new Material(CurShader);  
                CurMaterial.hideFlags = HideFlags.HideAndDontSave;  
            }  
            return CurMaterial;  
        }  
    }  

    void Start()  
    {  
        Instance = this;

        //找到当前的Shader文件  
        CurShader = Shader.Find("Custom/CameraMotionBlurShader");  

        //判断是否支持屏幕特效  
        if (!SystemInfo.supportsImageEffects)  
        {  
            EnableEffect = false;
            isTurnOn = false;
            return;  
        }  
    } 

    public void TurnOnEffect()
    {
        if (!EnableEffect)
            return;

        isTurnOn = true;
        IntensityChangeValue = 0;
        IntensityTemp = 0;
    }

    public void TurnOffEffect()
    {
        isTurnOn = false;
    }

    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)  
    {  
        //着色器实例不为空，就进行参数设置  
        if (CurShader != null && EnableEffect && IntensityChangeValue != 0)  
        {  
            //设置Shader中的外部变量  
            material.SetFloat("_IterationNumber", IterationNumber);  
            material.SetFloat("_Value", IntensityChangeValue);  
            material.SetFloat("_Value2", OffsetX);  
            material.SetFloat("_Value3", OffsetY);  
            material.SetVector("_ScreenResolution", new Vector4(sourceTexture.width, sourceTexture.height, 0.0f, 0.0f));  

            //拷贝源纹理到目标渲染纹理，加上我们的材质效果  
            Graphics.Blit(sourceTexture, destTexture, material);  
        }  
        //着色器实例为空，直接拷贝屏幕上的效果。此情况下是没有实现屏幕特效的  
        else  
        {  
            //直接拷贝源纹理到目标渲染纹理  
            Graphics.Blit(sourceTexture, destTexture);  
        }  

    }  

    void Update()  
    {  

        //找到对应的Shader文件  
        #if UNITY_EDITOR  
        if (Application.isPlaying != true)  
        {  
            CurShader = Shader.Find("Custom/CameraMotionBlurShader");  
        }  
        #endif 

        if (!EnableEffect)
        {
            return;
        }

        if (isTurnOn)
        {
            if (IntensityTemp < Intensity)
                IntensityTemp += IntensityChangeDelta;
            else
                IntensityTemp = Intensity;
        }
        else
        {
            if (IntensityTemp > 0)
                IntensityTemp -= IntensityChangeDelta;
            else
                IntensityTemp = 0;
        }

        IntensityChangeValue = IntensityTemp;
    }  


    void OnDisable()  
    {  
        if (CurMaterial)  
        {  
            DestroyImmediate(CurMaterial);  
        }  
    }  
}  