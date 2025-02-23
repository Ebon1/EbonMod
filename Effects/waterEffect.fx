sampler uImage0 : register(s0); // Main Render
sampler uImage1 : register(s1); // Main Noise
sampler uImage2 : register(s2); // Offsetting Noise
sampler uImage3 : register(s3); // Offsetting Noise 2

float time;
float totalAlpha = 1;

float2 mainDirection;
float2 secondaryDirection;
float2 tertiaryDirection;
float offset = 0.5f;
float mainScale = 1;
float secondaryScale = 1;
float tertiaryScale = 1;
float4 colOverride = float4(1, 1, 1, 1);

float2 Wrap(float2 uv)
{
    uv %= 1;
    uv += 1;
    uv %= 1;
    return uv;
}


float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 image = tex2D(uImage0, coords);

    float4 mainNoise = tex2D(uImage1, Wrap((coords + float2(mainDirection.x * time, mainDirection.y * time)) * mainScale));
    float4 secondaryNoise = tex2D(uImage2, Wrap((coords + float2(secondaryDirection.x * time, secondaryDirection.y * time)) * secondaryScale));
    float4 tertiaryNoise = tex2D(uImage3, Wrap((coords + float2(tertiaryDirection.x * time, tertiaryDirection.y * time)) * tertiaryScale));

    float4 col = min(mainNoise, min(secondaryNoise, tertiaryNoise));
    image = tex2D(uImage0, coords + offset * (max(col.r, max(col.g, col.b)) * 2 - 1));
    
    return float4(image.rgb, max(col.r, max(col.g, col.b))) * colOverride * totalAlpha;

}

technique Technique1
{
    pass waterEffect
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}