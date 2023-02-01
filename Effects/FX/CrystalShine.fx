sampler uImage0 : register(s0); // Contents of the screen.
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime; // Should be set to Main.GlobalTime or a fraction of incrementing time for shine effect.
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
	float4 colour = tex2D(uImage0, uv);

	float off = uv.x + uv.y * 0.2;
	float shine = clamp(sin((2.0 * uTime) - off * 15.0), 0.75, 1.0);

	colour.rgb *= shine;
	colour.a *= uOpacity;

	return colour;
}

technique Technique1
{
	pass DustifiedCrystalShinePass
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
