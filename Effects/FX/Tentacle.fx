sampler uImage0 : register(s0);

float4x4 uTransform;


struct VSInput
{
    float2 Pos : POSITION0;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};

struct PSInput
{
    float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};


float4 PixelShaderFunction(PSInput input) : COLOR0
{
    float a = mul(tex2D(uImage0, input.Texcoord), float3(1, 1, 1)) / 3;
    float4 c = tex2D(uImage0, input.Texcoord);
    if (input.Texcoord.x > 0.75)
    {
        a *= 1 - (input.Texcoord.x - 0.75) / 0.25;
    }
    return c * a;
}

PSInput VertexShaderFunction(VSInput input)
{
    PSInput output;
    output.Color = input.Color;
    output.Texcoord = input.Texcoord;
    output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
    return output;
}


technique Technique1
{
    pass Tentacle
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}