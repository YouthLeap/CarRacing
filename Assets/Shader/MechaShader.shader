// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//慎用，占了两个Pass的，要渲染两层东西，把透明的渲染上去
Shader "Custom/MechaShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimWidth ("Rim Width", Range(0,0.1)) = 0
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0
	}
	
	SubShader {
		Tags {"Queue"="Transparent"  "RenderType"="Transparent" }
	    ZWrite On Lighting Off Cull Off Blend SrcAlpha OneMinusSrcAlpha 
		LOD 100
		Pass
		{
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

            sampler2D _MainTex;
            uniform fixed4 _Color;
			uniform fixed4 _RimColor;
            float _RimWidth;
            fixed _Cutoff;
            
            struct appdata {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float2 texcoord : TEXCOORD0;
            };
			
			struct v2f {
			    float4 pos : SV_POSITION;
			    float4 uv : TEXCOORD0;
			    float3 viewDir : TEXCOORD1;
			    fixed4 color : COLOR;
			};
			
			

			v2f vert (appdata_full v)
			{
			   v2f o;

			   if(_RimWidth != 0)
			   {
				   //计算外发光的颜色占比
				   float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
		           float dotProduct = 1-dot(v.normal, viewDir);
		           o.color = smoothstep(1-_RimWidth, 1.0, dotProduct);
		           o.color *= _RimColor;
			   }
			   
			   o.pos = UnityObjectToClipPos (v.vertex);
			   o.uv = v.texcoord;
			   
			   return o;
			}
			half4 frag (v2f i) : COLOR0
			{
			  	half4 col = tex2D(_MainTex, i.uv);
			  	//透贴的效果剔除
			  	clip(col.a - _Cutoff);
			  	col *= _Color;
			  	//外发光的色彩叠加
			  	if(_RimWidth != 0)
			   	{
			  		col.rgb += (i.color.rgb * i.color.a);
			  	}

				return col;
			}
			ENDCG
		}

		Pass
		{
	    	ZWrite Off Lighting Off Cull Off Blend SrcAlpha OneMinusSrcAlpha 
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

            sampler2D _MainTex;
            uniform fixed4 _Color;
            fixed _Cutoff;
            
            struct appdata {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float2 texcoord : TEXCOORD0;
            };
			
			struct v2f {
			    float4 pos : SV_POSITION;
			    float4 uv : TEXCOORD0;
			    float3 viewDir : TEXCOORD1;
			    fixed4 color : COLOR;
			};
			
			

			v2f vert (appdata_full v)
			{
			   v2f o;
			   o.pos = UnityObjectToClipPos (v.vertex);
			   o.uv = v.texcoord;
			   
			   return o;
			}
			half4 frag (v2f i) : COLOR0
			{
			  	half4 col = tex2D(_MainTex, i.uv);
			  	clip(-(col.a - _Cutoff));
				return col;
			}
			ENDCG
		}
	}
	
	FallBack "Diffuse"
}
