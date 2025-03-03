sampler2D uImage0 : register(s0);

texture2D tex;
sampler2D uImage1 = sampler_state
{
    Texture = <tex>;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
bool wavinessMult = 1;
float scale = 2;
float intensity = 10;
float4 colOverride = float4(1, 0.25, 0, 1);
float uTime;

float4 Main(float2 fragCoord : TEXCOORD) : COLOR
{
    float _gradient = smoothstep(1, 0, fragCoord.y);
    float waviness = sin(fragCoord + uTime) * fragCoord.y * 0.05 * wavinessMult;
    float4 c = tex2D(uImage1, scale * (fragCoord + float2(0, -uTime * 0.9) + float2(waviness, 0)));
    float4 c2 = tex2D(uImage1, scale * ((-fragCoord + float2(1, 1)) + float2(0.5, 0.17) - float2(uTime * 0.31, -uTime * 1.17) - float2(waviness * 0.91, 0)));
    float4 mask = tex2D(uImage0, fragCoord + float2(waviness, 0));
    float maskA = max(mask.r, max(mask.g, mask.b)) * mask.a * 4;
    
    float a = maskA * smoothstep(0, 1, max(c.r, c2.r)) * intensity * _gradient;
    return c * c2 * a * colOverride;
}

Technique techique1
{
    pass FlameEffect
    {
        PixelShader = compile ps_3_0 Main();
    }
}