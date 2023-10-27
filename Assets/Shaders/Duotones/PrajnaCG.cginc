#ifndef PRAJNA_CG_INCLUDED
#define PRAJNA_CG_INCLUDED
			
inline half getChroma(fixed4 color)
{
	half mx = max(color.r, color.g);	mx = max(mx, color.b);
	half mn = min(color.r, color.g);	mn = min(mn, color.b);
	
	return mx - mn;
}

inline fixed4 GetColorPosition(fixed hue, fixed chroma)
{
	half h = hue * 6;
	half x = 1 - abs (h - 2*floor(h*0.5) - 1);
	
	fixed4 color = fixed4(1, 0, x, 1);
		
	if(h < 1)
		color = fixed4(1, x, 0, 1);
	else if(h < 2)
		color = fixed4(x, 1, 0, 1);
	else if(h < 3)
		color = fixed4(0, 1, x, 1);
	else if(h < 4)
		color = fixed4(0, x, 1, 1);
	else if(h < 5)
		color = fixed4(x, 0, 1, 1);
	
	color *= saturate(chroma);
	color.a = 1;

	return saturate(color);
}

inline fixed4 HCLtoRGB(fixed H, fixed C, fixed L)
{
	fixed4 hc = GetColorPosition(H, C);
	fixed currLuma = hc.r * 0.299 + hc.g * 0.587 + hc.b * 0.114;
	fixed modifier = L - currLuma;				
	fixed4 color = hc + modifier;
	
	return saturate(color);
}

inline fixed4 HSLtoRGB(fixed H, fixed S, fixed L)
{
	half chroma = (1-abs(2*L - 1)) * S;
	fixed4 hc = GetColorPosition(H, chroma);
	fixed modifier = L - 0.5f*chroma;
	fixed4 color = hc + modifier;
	
	return color;
}

inline fixed4 HSVtoRGB(fixed H, fixed S, fixed V)
{
	half chroma = V*S;
	fixed4 hc = GetColorPosition(H, chroma);
	fixed modifier = V - chroma;				
	fixed4 color = hc + modifier;
	
	return color;
}

#endif