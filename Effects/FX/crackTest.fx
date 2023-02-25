sampler2D spriteTexture : register(s0);

float4 PS_Main(float2 texCoord : TEXCOORD0) : COLOR
{
    // Sample the sprite texture at the given texture coordinate
    float4 spriteColor = tex2D(spriteTexture, texCoord);

    // Apply a simple cracking effect
    float noise = tex2D(spriteTexture, texCoord * 5).r;
    float crackIntensity = saturate((noise - 0.5) * 10);
    float4 crackColor = float4(1, 1, 1, 0) * crackIntensity;
    float4 result = lerp(spriteColor, crackColor, crackIntensity);

    return result;
}
Technique techique1
{
    pass Main
    {
        PixelShader = compile ps_2_0 PS_Main();
    }
}