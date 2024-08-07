sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2); //tex
sampler uImage3 : register(s3); //noise
sampler uImage4 : register(s4);
float uTime;
float offsetX = 1, offsetY = 1;
float noiseThreshold = 0.5f;
float2 Wrap(float2 uv)
{
    uv %= 1;
    uv += 1;
    uv %= 1;
    return uv;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage0, coords);
    float a = max(c.r, max(c.g, c.b));
    float4 c2 = tex2D(uImage1, float2(a, 0));
    float4 c3 = tex2D(uImage2, coords);
    float4 c4 = tex2D(uImage4, float2(a, 0));
    float2 noiseCoords = coords;
    float2 noiseCoords2 = coords;
    float2 noiseCoords3 = coords;
    float2 noiseCoords4 = coords;
    
    noiseCoords.x += uTime * offsetX;
    noiseCoords2.y -= uTime * offsetY * 0.25f;
    noiseCoords2.x -= uTime * offsetX;
    noiseCoords3.y += uTime * offsetY * 0.16f;
    noiseCoords3.x += uTime * offsetX * 0.51f;
    noiseCoords4.y -= uTime * offsetY * 0.26f;
    noiseCoords4.x -= uTime * offsetX * 0.61f;
    
    noiseCoords = Wrap(noiseCoords);
    noiseCoords2 = Wrap(noiseCoords2);
    noiseCoords3 = Wrap(noiseCoords3);
    noiseCoords4 = Wrap(noiseCoords4);
    float4 noise1 = tex2D(uImage3, noiseCoords);
    float4 noise2 = tex2D(uImage3, noiseCoords2);
    float4 noise3 = tex2D(uImage3, noiseCoords3);
    float4 noise4 = tex2D(uImage3, noiseCoords4);
    float noiseA = max(noise1.r, noise2.r) - min(noise1.r, noise2.r);
    float noiseA2 = max(noise3.r, noise4.r) - min(noise3.r, noise4.r);
    a *= clamp(-noiseA2 + 1, 0, 1);

    return lerp(c2 * a, float4(c3.rgb, a), noiseA * a);
}

technique Technique1
{
    pass metaballGradientNoiseTex
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}