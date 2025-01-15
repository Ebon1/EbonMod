sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float m;
float n;
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage0, coords);
    float a = max(c.r, max(c.g, c.b));
    if (a > m)
    {
        float4 c1 = tex2D(uImage1, coords);
        return c1;
    }
    else if (abs(a - m) < n)
        return float4(2.55, 2.48, .59, 1); //set the color here, its currently orange here but you can probably like make a variable to make custom colors (this is for the edges)
    else
        return c * a;
}

technique Technique1
{
    pass LavaRT
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
   
}