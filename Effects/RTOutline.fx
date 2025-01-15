sampler uImage0 : register(s0);
sampler uImage1;
float m;
float n;
float4 col = float4(2.55, 2.55, 2.55, 1);
float4 colStart = float4(2.55, 2.55, 2.55, 1);
bool colAlphaAffectsMainImage = false;
bool secondary = true;
float lerpSpeed = 5;
float2 offset = float2(0, 0);
bool useMainAlpha = true;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage0, coords);
    float a = max(c.r, max(c.g, c.b));
    float alpha = lerp(colStart.a, col.a, abs(a - m) * lerpSpeed);
    float alpha2 = c.a;
    if (!useMainAlpha)
        alpha2 = 1;
    if (a > m && secondary)
    {
        float4 c1 = tex2D(uImage1, coords + offset);
        if (colAlphaAffectsMainImage)
            return c1 * alpha;
        else
            return c1;
    }
    else if (abs(a - m) < n)
        return lerp(colStart, col, abs(a - m) * lerpSpeed) * alpha2;
    else
    {
        if (colAlphaAffectsMainImage)
            return c * a * alpha * alpha2;
        else
            return c * a * alpha2;
    }
}

technique Technique1
{
    pass RTOutline
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}