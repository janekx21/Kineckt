// This shader is a post processing shader
// it makes a few color changes and blurs the screen edges
// @author Janek Winkler

sampler ViewTexture;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

// horizontal blur effect
float3 blur(sampler tex, float2 uv, float strength)
{
    float3 color = float3(0, 0, 0);
	for(int i = 0; i < 5; i++) {
        color += tex2D(tex, uv + .005 * strength * i * float2(0, 1)).rgb;
	}
	color /= 6;
	return color;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(ViewTexture, input.TextureCoordinates);
	
	float strength = pow(length((input.TextureCoordinates - .5 + float2(0, .2)) * float2(1, .5)) * 2.5, 5);
    color.rgb = blur(ViewTexture, input.TextureCoordinates, strength);
	
	color.rgb = color.rgb * 1.1 + .01; // contrast
	
	float adjustment = 3;
	float3 W = float3(0.2125, 0.7154, 0.0721);
    float intensity = dot(color.rgb, W);
    color.rgb = lerp(intensity, color.rgb, adjustment); // saturation effect
	
	return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile ps_5_0 MainPS();
	}
};