// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Prajna/Duotone/HCL"
{
	Properties
	{
		_Hue ("Hue Value", Range(0, 1)) = 0
		_HZero ("Hue Zero", Range(0, 1)) = 0
		_HRatio ("Hue Ratio", Range(0, 2)) = 0		
		
		_Chroma ("Chroma Value", Range(0, 1)) = 0.5
		_CZero ("Chroma Zero", Range(0, 1)) = 0
		_CRatio ("Chroma Ratio", Range(0, 2)) = 0		
		
		_Luma ("Luma Valule", Range(0, 1)) = 0.5
		_LZero ("Luma Zero", Range(0, 1)) = 0
		_LRatio ("Luma Ratio", Range(0, 2)) = 1		
		
		_Map ("Luma Map", 2D) = "white" {}		
		_Alpha ("Alpha", Range(0, 1)) = 1
		
		_OffsetY ("Scroll" , float) = 0
		_TilingY ("Tiling", float) = 1		
	}	
	
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha 
		
		Pass
    	{
	    	CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
				
			#include "UnityCG.cginc"
			#include "PrajnaCG.cginc"
	
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};
			
			float _Hue;
			float _HZero;
			float _HRatio;			
			
			float _Chroma;
			float _CZero;
			float _CRatio;			
			
			float _Luma;
			float _LZero;
			float _LRatio;			
			
			sampler2D _Map;
			float _Alpha;			
			
			float _OffsetY;
			float _TilingY;					
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.texcoord.y += _OffsetY;
				o.texcoord.y *= _TilingY;
				return o;
			}
				
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 map = tex2D(_Map, i.texcoord);
				fixed H = (_Hue + (map.r - _HZero)*_HRatio) + 1;
				H = H - floor(H);
				fixed C = saturate(_Chroma + (map.g - _CZero)*_CRatio);
				fixed L = saturate(_Luma + (map.b - _LZero) * _LRatio);
				fixed4 color = HCLtoRGB(H, C, L);
				color.a = map.a * _Alpha;
				
				return saturate(color);
			}
			ENDCG
    	}
	}	
	SubShader
	{
		Tags
		{
			"Queue" = "Background"
			"IgnoreProjector" = "True"
			"RenderType" = "Opaque"
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha 
		
		Pass
		{
			SetTexture [_Map]
       		{
           		Combine texture
       		}
		}
	}
}
