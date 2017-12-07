#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
Texture2D SpriteNormal;
Texture2D SpriteNormal2;
//float3 LightDirection = { 1.0,0.0,1.0 };
float3 LightDirection = { 0.0,1.0,1.0 };
float3 LightColor = 1.0;
float3 AmbientColor = 0.5;
matrix WorldViewProjection;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

sampler2D SpriteNormalSampler  = sampler_state
{
	Texture = <SpriteNormal>;
};

sampler2D SpriteNormalSampler2  = sampler_state
{
	Texture = <SpriteNormal2>;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 newColour = tex2D(SpriteTextureSampler, input.TextureCoordinates);
	float4 normal = normalize(tex2D(SpriteNormalSampler, input.TextureCoordinates) + tex2D(SpriteNormalSampler2, input.TextureCoordinates) - 1.0);
	float lightAmount = dot(normal.xyz, LightDirection);
	if (lightAmount < 0.85) lightAmount /= 2.0;
	newColour.xyz += (lightAmount * 1.0 * LightColor);
	newColour.a = tex2D(SpriteTextureSampler, input.TextureCoordinates).a * lightAmount;
	return newColour;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};