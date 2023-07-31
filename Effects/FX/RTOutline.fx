sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float m;
float n;
float4 col = float4(2.55, 2.55, 2.55, 1);
bool secondary = true;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage0, coords);
    float a = max(c.r, max(c.g, c.b));
    if (a > m && secondary)
    {
        float4 c1 = tex2D(uImage1, coords);
        return c1 * col.a;
    }
    else if (abs(a - m) < n)
        return col;
    else
        return c * a * col.a;
}

technique Technique1
{
    pass RTOutline
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}