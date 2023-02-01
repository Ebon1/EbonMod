sampler uImage0 : register(s0);

float uScale;
float2 uOffset;

texture maskTexture;
sampler mask = sampler_state 
{
    Texture = maskTexture;
};

texture overlayTexture;
sampler overlay = sampler_state 
{
    Texture = overlayTexture;
};

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0 
{
    float4 color = tex2D(uImage0, coords);
    float4 maskColor = tex2D(mask, coords);

    if (!any(maskColor)) 
    {
        return color;
    }

    return tex2D(overlay, ((coords + uOffset) / uScale) % 1.0);
}

technique Technique1 
{
    pass TextureMaskPass 
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}