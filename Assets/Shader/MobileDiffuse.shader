//根据mobile/Diffuse的源码改编，目的为了增加颜色跟亮度控制
Shader "Custom/MobileDiffuse" {
	Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Brightness ("Brightness", Float) = 1
	_Color ("Color", Color) = (1,1,1,1)
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 150

CGPROGRAM
#pragma surface surf Lambert noforwardadd

sampler2D _MainTex;
fixed4 _Color;
fixed _Brightness;

struct Input {
	fixed2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color * _Brightness;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "Mobile/VertexLit"
}
