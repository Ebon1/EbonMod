sampler2D texSampler : register(s0);

float4 main(float2 texCoord : TEXCOORD) : COLOR
{
    float blurAmount = 0.01; // Set the blur amount here
    float blurSize = 0.005; // Set the blur size here

    float4 color = 0;
    float weightSum = 0;

    // Sample the color for the current pixel and its neighboring pixels
    for (int i = -4; i <= 4; i++)
    {
        float2 offset = float2(i * blurSize, 0.0);
        float4 sampleColor = tex2D(texSampler, texCoord + offset);
        float weight = 0.0;

        if (i == 0)
        {
            weight = 0.16;
        }
        else if (abs(i) == 1)
        {
            weight = 0.15;
        }
        else if (abs(i) == 2)
        {
            weight = 0.12;
        }
        else if (abs(i) == 3)
        {
            weight = 0.09;
        }
        else if (abs(i) == 4)
        {
            weight = 0.05;
        }

        color += sampleColor * weight;
        weightSum += weight;
    }

    // Normalize the color by the total weight to get the blurred color
    color /= weightSum;

    // Return the blurred color
    return color / blurAmount;
}

technique Technique1
{
    pass blur
    {
        PixelShader = compile ps_2_0 main();
    }
}