// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/FangxiangpaiUnlitShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Len("Len",Range(0,0.3))=0.1
		_Speed("Speed",Float)=1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque"  "Queue"="Transparent" "IgnoreProjector"="True"}
		LOD 100
		ZWrite Off Cull Off
	   Blend SrcAlpha OneMinusSrcAlpha 

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
		    float _Len;
		    float _Speed;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);		
				
				float begin=_Time*_Speed;
				begin = fmod(begin,1);
				
				if(begin< i.uv.x  && i.uv.x<begin+_Len && col.a>0.5)
				{
				    col.rgb = float3(1,1,1);
				    col.a=0.9;
				}		
						
				return col;
			}
			ENDCG
		}
	}
}
