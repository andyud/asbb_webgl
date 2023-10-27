Shader "Prajna/Transparent Multiply Colored"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}

	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 
		Alphatest Greater 0
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
