sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
float m;
float n;
float4 col = float4(2.55, 2.55, 2.55, 1);
bool useAlphaGradient = false;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage0, coords);
    float a = max(c.r, max(c.g, c.b));
    float4 c2 = tex2D(uImage1, float2(a, 0));
    float4 c3 = tex2D(uImage2, float2(a, 0));
    if (abs(a - m) < n && !useAlphaGradient)
        return col;
    if (useAlphaGradient)
        return c2 * c3.a * a;
    return c2 * a;
}

technique Technique1
{
    pass metaballGradient
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}