sampler2D tex : register(s0);
float4 color2;
float4 color3;
float amount;
float4 Main(float2 coords : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(tex, coords);
	float distfromcenter=distance(float2(0.5f, 0.5f), coords);
	return lerp(color2, color3, (coords.x - sin(amount)) * 0.5f) * color.a;
}

Technique techique1
{
	pass Galaxy
	{
		PixelShader = compile ps_2_0 Main();
	}
}
