// Global Variables
float4x4 world : WORLD;
float4x4 wvp : WorldViewProjection;

float3 lightDirection;

float3 color = 1;

texture textureMat;
sampler textureSample = sampler_state 
{
    texture = <textureMat>; 
	AddressU = Wrap;
	AddressV = Wrap;   
	MipFilter = LINEAR; 
	MinFilter = LINEAR; 
	MagFilter = LINEAR; 
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexCoord : TexCoord0;
	float3 Normal	: Normal0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TexCoord0;
    float3 Normal : Normal0;
	float3 ld : Normal1;
	float4 SPos : TexCoord1;
};

struct PixelShaderOutput
{
	float4 Color : COLOR0;
	float4 Depth : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    
    output.Position = mul(input.Position, wvp);
    output.TexCoord = input.TexCoord;
        
    output.Normal = mul(-input.Normal,world);

	output.ld = normalize(lightDirection);
	
	output.SPos = output.Position;
    
    return output;
}

PixelShaderOutput PSBasicTexture(VertexShaderOutput input) : COLOR0
{
	PixelShaderOutput output = (PixelShaderOutput)0;
	
	output.Color = tex2D(textureSample,input.TexCoord) * (saturate(dot(input.ld,input.Normal)) + .25f);  
		
	// Depth
	output.Depth.r = 1-(input.SPos.z/input.SPos.w); // Flip to keep accuracy away from floating point issues.
	output.Depth.a = 1;
	
	return output;
}

technique Deferred
{
    pass Pass1
    {
		VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PSBasicTexture();
    }
}
