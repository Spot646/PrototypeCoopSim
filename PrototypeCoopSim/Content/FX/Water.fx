#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
const float4 alphaColor = { 0,0,0,0 };
const float4 alphaReplaceColor = { 1,1,1,1 };

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 PSHorizontalBlur(VertexShaderOutput input) : COLOR
{
	float4 total = { 0.0,0.0,0.0,0.0 }; 
	float4 colourCheck;
	float2 xShift = { 1.0 / 1024.0, 0.0 / 768};
	int weights[11] = { 1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1 };
	int weightSum = 0;
	for (int i = 1; i < 11; ++i)
	{
		//assume entire screen is [0,1]
		float2 tCord = float2(input.TextureCoordinates.x + xShift.x * (i - 7), input.TextureCoordinates.y);
		colourCheck = tex2D(SpriteTextureSampler, tCord);
		if (colourCheck.r > 0.0) 
		{
			total += colourCheck * weights[i];
			weightSum += weights[i];
		}		
	}	
	total /= weightSum;
	return float4(total.rgb, tex2D(SpriteTextureSampler, input.TextureCoordinates).a);
}

float4 PSVerticalBlur(VertexShaderOutput input) : COLOR
{
	float4 total = { 0.0,0.0,0.0,0.0 };
	float4 colourCheck;
	float2 yShift = { 1.0 / 1024.0, 1.0 / 768 };
	int weights[11] = { 1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1 };
	int weightSum = 0;
	for (int i = 1; i < 11; ++i)
	{
		//assume entire screen is [0,1]
		float2 tCord = float2(input.TextureCoordinates.x, input.TextureCoordinates.y + yShift.y * (i - 7));
		colourCheck = tex2D(SpriteTextureSampler, tCord);
		if (colourCheck.r > 0.0)
		{
			total += colourCheck * weights[i];
			weightSum += weights[i];
		}
	}
	total /= weightSum;
	return float4(total.rgb, tex2D(SpriteTextureSampler, input.TextureCoordinates).a);
}

technique SpriteDrawingHorizontalBlur
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL PSHorizontalBlur();
	}
};

technique SpriteDrawingVerticalBlur
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL PSVerticalBlur();
	}
};