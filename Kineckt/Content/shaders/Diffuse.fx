// You can only upload uniforms that you also use in code

matrix World;
matrix View;
matrix Projection;
matrix WorldInverse;
matrix ViewInverse;
matrix ProjectionInverse;

sampler Alberto;

sampler Shadow;
matrix ShadowProjection;

struct Diffuse_VSOut
{
    float4 Position : POSITION;
    float2 UV :TEXCOORD0;
    float4 ShadowPosition : TEXCOORD2;
};

//  CREATE SHADOW MAP
Diffuse_VSOut Diffuse_VertexShader(float4 Position : SV_POSITION, float2 UV : TEXCOORD0)
{
    Diffuse_VSOut Out;
    Out.Position = mul(Position, mul(World, mul(View, Projection)));
    Out.ShadowPosition = mul(Position, ShadowProjection);
    Out.UV = UV;
    
    return Out;
}

float4 Diffuse_PixelShader(Diffuse_VSOut input) : COLOR
{
    float4 color = tex2D(Alberto, input.UV);
    
    float4 p = input.ShadowPosition;
    p /= p.w;
    p = p * .5 ;
    p += .5; 
    p.y *= -1;
    float shadowDepth = tex2D(Shadow, p.xy).r;
    float depth = input.ShadowPosition.z / input.ShadowPosition.w;
    float shadow = smoothstep(.002, .00001, depth - shadowDepth);
    color.rgb = lerp(color.rgb * shadow, color.rgb, .6);
    
    return color;
}

// Technique for creating the shadow map
technique Diffuse
{
    pass Pass1
    {
        VertexShader = compile vs_5_0 Diffuse_VertexShader();
        PixelShader = compile ps_5_0 Diffuse_PixelShader();
    }
}
