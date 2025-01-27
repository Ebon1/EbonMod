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
float threshhold = 0.4;
float2 uResolution;

float4 PixelShaderFunction(float2 coords : TEXCOORD) : COLOR
{
    float4 col = tex2D(uImage0, coords);
    if (max(col.r, max(col.g, col.b)) < threshhold)
    {
        return col;
    }
    float4 blur = col;
    for (float i = -3; i < 3; i++)
    {
        for (float j = -3; j < 3; j++)
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