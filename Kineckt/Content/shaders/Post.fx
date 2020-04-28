sampler ViewTexture;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(ViewTexture, input.TextureCoordinates); //tex2D(s0, input.TextureCoordinates);
	
	
	float strength = pow(abs(input.TextureCoordinates.x - .5) * 2, 4);
	for(int i=0;i<5;i++) {
        color.rgb += tex2D(ViewTexture, input.TextureCoordinates + float2(0, 1) *i*.005 * strength).rgb;
	}
	color.rgb /= 6;
	
	color.rgb = color.rgb*1.1 + .01;
	
	float adjustment = 3;
	float3 W = float3(0.2125, 0.7154, 0.0721);
    float intensity = dot(color.rgb, W);
    color.rgb = lerp(intensity, color.rgb, adjustment);
	
	return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile ps_5_0 MainPS();
	}
};