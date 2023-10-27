Shader "Prajna/Multiply Colored"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}

	SubShader
	{
		Tags {"Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque"}
		ZWrite Off
		Pass
		{			
			Fog { Mode Off }			
			Lighting Off
       		SetTexture [_MainTex]
       		{
           		constantColor [_Color]
           		Combine texture * constant
       		}
		}
	} 
}
