// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PlantShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("贴图颜色", Color) = (1,1,1,1)
        _Cutoff ("透明度剔除", Range(0,1)) = 0
        [Enum(CullMode)]_CullMode("剔除模式", Float) = 0
	}
	
	SubShader {
		Tags {"RenderType"="Opaque" }
	    ZWrite On Lighting Off Cull [_CullMode] Blend SrcAlpha OneMinusSrcAlpha 
		LOD 100
		Pass
		{
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
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
				UNITY_FOG_COORDS(3)
			};

			v2f vert (appdata_full v)
			{
			   v2f o;
			   o.pos = UnityObjectToClipPos (v.vertex);
			   o.uv = v.texcoord;
			   UNITY_TRANSFER_FOG(o,o.pos);
			   return o;
			}
			half4 frag (v2f i) : COLOR0
			{
			  	half4 col = tex2D(_MainTex, i.uv);
			  	//透贴的效果剔除
			  	clip(col.a - _Cutoff);
			  	col *= _Color;
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
	
	FallBack "Diffuse"
}
