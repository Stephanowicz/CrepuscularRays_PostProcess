#include "PPVertexShader.fxh"
uniform extern float BloomThreshold;

float2 halfPixel;
sampler TextureSampler : register(s0);


float4 BrightPassPS(float2 texCoord : TEXCOORD0) : COLOR0
{
	texCoord -= halfPixel;
    // Look up the original image color.
    float4 c = tex2D(TextureSampler, texCoord);

    // Adjust it to keep only values brighter than the specified threshold.
    return saturate((c - BloomThreshold) / (1 - BloomThreshold));
}


technique BloomExtract
{
    pass P0
    {
		VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 BrightPassPS();
    }
}