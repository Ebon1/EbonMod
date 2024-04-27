sampler uImage0 : register(s0);
texture2D tex0;
sampler2D uImage1 = sampler_state
{
    Texture = <tex0>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage0, coords);
    float _a = max(c.r, max(c.g, c.b));
    float4 c2 = tex2D(uImage1, coords);
    float a = lerp(1, 0, c2.a);

    return c * _a * a;
}

technique Technique1
{
    pass invisibleMask
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}