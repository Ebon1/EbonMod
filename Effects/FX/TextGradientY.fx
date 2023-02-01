sampler uImage0 : register(s0);

float4 color2;
float4 color3;
float amount;
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
    float a = tex2D(uImage0, coords).a;


    return lerp(color2, color3, (coords.y - sin(amount)) * 0.5f) * a;
}

technique Technique1 
{
    pass GlitchPass 
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}