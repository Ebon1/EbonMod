sampler uImage0 : register(s0);

float uTime;
float uOpacity;
float uIntensity;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0 
{
    float4 color = tex2D(uImage0, coords);

    if (!any(color))
    {
        return color;
    }

    float red = tex2D(uImage0, coords + float2(-uIntensity * coords.x, uIntensity * coords.y)).r;
    float green = tex2D(uImage0, coords).g;
    float blue = tex2D(uImage0, coords + float2(uIntensity * coords.x, -uIntensity * coords.y)).b;

    float4 glitch = float4(red, green, blue, uOpacity);

    return glitch;
}

technique Technique1 
{
    pass GlitchPass 
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}