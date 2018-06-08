#include "PPVertexShader.fxh"

float3 lightPosition;

float4x4 matVP;
float4x4 matInvVP;

float2 halfPixel;


texture scene;
sampler2D Scene
{
    Texture = <scene>;
    AddressU = Mirror;
	AddressV = Mirror;
};

texture depthMap;
sampler2D DepthMap = sampler_state
{
	Texture = <depthMap>;
	MinFilter = Point;
	MagFilter = Point;
	MipFilter = None;
};

float4 LightSourceSceneMaskPS(float2 texCoord : TEXCOORD0) : COLOR0
{
	float depthVal = 1 - (tex2D(DepthMap, texCoord).r);

	float4 scene = tex2D(Scene,texCoord);
	float4 position;
	position.x = texCoord.x * 2.0f - 1.0f;
	position.y = -(texCoord.y * 2.0f - 1.0f);
	position.z = depthVal;
	position.w = 1.0f;
	
	// Pixel pos in the world
	float4 worldPos = mul(position, matInvVP);
	worldPos /= worldPos.w;

	// Find light pixel position
	float4 ScreenPosition = mul(lightPosition, matVP);
	ScreenPosition.xyz /= ScreenPosition.w;
	ScreenPosition.x = ScreenPosition.x/2.0f+0.5f;
	ScreenPosition.y = (-ScreenPosition.y/2.0f+0.5f);

	// If the pixel is infront of the light source, blank it out..
	if(depthVal < ScreenPosition.z - .00025)
		scene = 0;

	return scene;
}


technique LightSourceSceneMask
{
	pass p0
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 LightSourceSceneMaskPS();
	}
}