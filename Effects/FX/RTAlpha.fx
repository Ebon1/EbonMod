sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float m;
float n; //useless
texture noiseTex;
sampler2D noiseTexture : sampler_2D
{
	Texture = (noiseTex);
	AddressU = Wrap;
	AddressV = Wrap;
};
float2 screenPosition;
float2 screenSize;
float distortionMultiplier;
float uTime;
float alpha;

float2 Wrap(float2 uv)
{
	uv %= 1;
	uv += 1;
	uv %= 1;
	return uv;
}
float4 MainPS(float2 uv : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage0,uv);
	float a=max(c.r,max(c.g,c.b));
	float2 center = float2(0.5, 0.5);
	float2 dir = uv - center;
    float len = length(dir);
    float rot = atan2(dir.x, dir.y);
    float offset = tex2D(noiseTexture, uv) - sin(uTime);
    rot += offset * 0.025 * distortionMultiplier;
	uv = center + float2(sin(rot), cos(rot)) * len;
	
	uv -= screenPosition / screenSize;
	uv = Wrap(uv);
    if(a>m)
	{
		float4 c1=tex2D(uImage1,uv);
		return c1 * c.a;
	}
	else 
	{
		float4 c1=tex2D(uImage1,uv);
		return c1 * c.a;
	}
}

Technique technique1
{
	pass Main
	{
		PixelShader = compile ps_2_0 MainPS();
	}
}