#include "PPVertexShader.fxh"

float3 lightPosition;


float4x4 matVP;

float2 halfPixel;

float SunSize = 1500;

texture scene;
sampler2D Scene
{
    Texture = <scene>;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture flare;
sampler Flare = sampler_state
{
    Texture = (flare);
    AddressU = CLAMP;
    AddressV = CLAMP;
};

float4 LightSourceMaskPS(float2 texCoord : TEXCOORD0 ) : COLOR0
{
	texCoord -= halfPixel;

	// Get the scene
	float4 col = 0;
	
	// Find the suns position in the world and map it to the screen space.
	float4 ScreenPosition = mul(lightPosition,matVP);
	float scale = ScreenPosition.z;
	ScreenPosition.xyz /= ScreenPosition.w;
	ScreenPosition.x = ScreenPosition.x/2.0f+0.5f;
	ScreenPosition.y = (-ScreenPosition.y/2.0f+0.5f);

	// Are we lokoing in the direction of the sun?
	if(ScreenPosition.w > 0)
	{		
		float2 coord;
		
		float size = SunSize / scale;
					
		float2 center = ScreenPosition.xy;

		coord = .5 - (texCoord - center) / size * .5;
		col += (pow(tex2D(Flare,coord),2) * 1) * 2;						
	}
	
    return col;// * tex2D(Scene, texCoord);
}

technique LightSourceMask
{
	pass p0
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 LightSourceMaskPS();
	}
}