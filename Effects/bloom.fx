sampler2D uImage0 : register(s0);

texture2D tex0;
sampler2D uImage1 = sampler_state
{
    Texture = <tex0>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

float distance = 15;
float intensity = 0.005;
float2 uResolution;

float4 PixelShaderFunction(float2 coords : TEXCOORD) : COLOR
{
    float4 col = tex2D(uImage0, coords);
    
    float4 blur = col;
    for (float i = -8; i < 8; i++)
    {
        for (float j = -8; j < 8; j++)
        {
            blur = blur + tex2D(uImage0, coords + (float2(i / 2.0, j / 2.0) * distance));
        }
    }
    blur *= intensity;
    
    return col + blur;
}

technique Technique1
{
    pass bloom
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}