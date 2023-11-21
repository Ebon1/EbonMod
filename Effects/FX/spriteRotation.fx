sampler2D tex : register(s0);
float rotation;
float2 scale = float2(1, 1);
float4 main(float2 uv : TEXCOORD) : COLOR
{
    float2 center = float2(0.5, 0.5);
    float2 dir = (uv - float2(0.5, 0.5)) / scale;
    float len = length(dir);
    float rot = atan2(dir.y, dir.x) + rotation;
    
    uv = center + float2(sin(rot), cos(rot)) * len;
    if (uv.x > 1 || uv.x < 0 || uv.y > 1 || uv.y < 0)
    {
        return float4(0, 0, 0, 0);
    }
    float4 color = tex2D(tex, uv);
    return color;
}
Technique techique1
{
    pass Rotation
    {
        PixelShader = compile ps_2_0 main();
    }
}
