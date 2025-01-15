sampler2D tex : register(s0);
texture galaxy;
sampler2D galaxySampler : sampler_2D
{
	Texture = (galaxy);
	AddressU = Wrap;
	AddressV = Wrap;
};

float4 Main(float2 coords : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(tex, coords);
	float4 g = tex2D(galaxySampler, coords);
	return g * color.a;
}

Technique techique1
{
	pass Galaxy
	{
		PixelShader = compile ps_2_0 Main();
	}
}