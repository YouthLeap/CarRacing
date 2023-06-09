﻿//根据mobile/Unlit的源码改编，目的为了增加颜色跟亮度控制
Shader "Custom/MobileUnlit" {
	Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Color ("Color", Color) = (1,1,1,1)
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 100
	
	// Non-lightmapped
	Pass {
		Tags { "LightMode" = "Vertex" }
		Lighting Off
		SetTexture [_MainTex] {
			constantColor [_Color]
			combine texture * constant DOUBLE// UNITY_OPAQUE_ALPHA_FFP
		}  
	}
//	
//	// Lightmapped, encoded as dLDR
//	Pass {
//		Tags { "LightMode" = "VertexLM" }
//
//		Lighting Off
//		BindChannels {
//			Bind "Vertex", vertex
//			Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
//			Bind "texcoord", texcoord1 // main uses 1st uv
//		}
//		
//		SetTexture [unity_Lightmap] {
//			matrix [unity_LightmapMatrix]
//			combine texture
//		}
//		SetTexture [_MainTex] {
//			constantColor (1,1,1,1)
//			combine texture * previous DOUBLE, constant // UNITY_OPAQUE_ALPHA_FFP
//		}
//	}
//	
//	// Lightmapped, encoded as RGBM
//	Pass {
//		Tags { "LightMode" = "VertexLMRGBM" }
//		
//		Lighting Off
//		BindChannels {
//			Bind "Vertex", vertex
//			Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
//			Bind "texcoord", texcoord1 // main uses 1st uv
//		}
//		
//		SetTexture [unity_Lightmap] {
//			matrix [unity_LightmapMatrix]
//			combine texture * texture alpha DOUBLE
//		}
//		SetTexture [_MainTex] {
//			constantColor (1,1,1,1)
//			combine texture * previous QUAD, constant // UNITY_OPAQUE_ALPHA_FFP
//		}
//	}	
//	

}
}
