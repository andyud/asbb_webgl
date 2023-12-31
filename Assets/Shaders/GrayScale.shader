﻿Shader "Prajna/GrayScale"
{
	Properties
	{	
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_LuminosityAmount ("GrayScale Amount", Range(0.0, 1)) = 0
		_LuminosityRatio ("GrayScale Ratio", Range(0, 2)) = 1
		_LuminosityShift ("GrayScale Shift", Range(-1, 1)) = 0
	}
	
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			fixed _LuminosityAmount;
			fixed _LuminosityRatio;
			fixed _LuminosityShift;
			
			fixed4 frag(v2f_img i) : COLOR
			{
				fixed4 renderTex = tex2D(_MainTex, i.uv);
				fixed luminosity = 0.299 * renderTex.r + 0.587 * renderTex.g + 0.114 * renderTex.b;
				luminosity = luminosity * _LuminosityRatio + _LuminosityShift;	
				fixed4 finalColor = lerp(renderTex, luminosity, _LuminosityAmount);
				
				return finalColor;
			}
			ENDCG
		}
	} 
	FallBack off
}
