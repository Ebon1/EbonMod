struct PSInput
{
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
    float4 Position : SV_Position;
};
struct VSInput
{
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
    float4 Position : POSITION;
};
sampler2D texSampler : register(s0);

matrix WorldViewProjection;
PSInput MainVS(VSInput input)
{
    PSInput output = (PSInput) 0;
    output.Color = input.Color;
    output.TexCoord = input.TexCoord;
    output.Position = mul(input.Position, WorldViewProjection);
    
    return output;
}

float4 MainPS(PSInput input) : COLOR0
{
    return input.Color;
}
float4 TexturePS(PSInput input) : COLOR0
{
    return input.Color * tex2D(texSampler, input.TexCoord).r;
}

technique Technique1
{
    pass Default
    {
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 MainPS();
    }
    pass Texture
    {
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 TexturePS();
    }
}