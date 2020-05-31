// This is a unlit shader
// it will render the Color strait to the frame buffer
// @author Janek Winkler

matrix World;
matrix View;
matrix Projection;

float4 Color;

struct Unlit_VSOut
{
    float4 Position : POSITION;
};

Unlit_VSOut Unlit_VertexShader(float4 Position : SV_POSITION)
{
    Unlit_VSOut Out;
    Out.Position = mul(Position, mul(World, mul(View, Projection)));
    return Out;
}

float4 Unlit_PixelShader(Unlit_VSOut input) : COLOR
{
    return float4(Color.rgb, 1);
}

// Technique for creating the shadow map
technique Unlit
{
    pass Pass1
    {
        VertexShader = compile vs_5_0 Unlit_VertexShader();
        PixelShader = compile ps_5_0 Unlit_PixelShader();
    }
}
