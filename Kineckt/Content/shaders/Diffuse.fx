// This is a diffuse shader
// @author Janek Winkler

matrix World;
matrix View;
matrix Projection;

sampler Alberto;
sampler AmbientOcclusion;

texture Shadow;
sampler ShadowSampler = sampler_state
{
    Texture = <Shadow>;
    AddressU = Clamp;
    AddressV = Clamp;
};
matrix ShadowProjection;

float3 LightDirection;
float3 CameraPosition;

struct Diffuse_VSOut
{
    float4 Position : POSITION;
    float2 UV :TEXCOORD0;
    float4 WorldPosition : TEXCOORD1;
    float4 ShadowPosition : TEXCOORD2;
    float3 Normal : NORMAL;
};

Diffuse_VSOut Diffuse_VertexShader(float4 Position : SV_POSITION, float2 UV : TEXCOORD0, float3 Normal : NORMAL)
{
    Diffuse_VSOut Out;
    
    Out.Position = mul(Position, mul(World, mul(View, Projection)));
    Out.ShadowPosition = mul(Position, ShadowProjection);
    Out.WorldPosition = mul(Position, World);
    Out.Normal = mul(float4(Normal, 0), World);
    Out.UV = UV;
    
    return Out;
}

// ambient light amount
float ambient()
{
    return .4;
}

// diffuse light amount
float diffuse(float3 normal, float3 lightDirection)
{
    return max(dot(-lightDirection, normal), 0);
}

// specular light amount
float light(float3 lookDirection, float3 normal, float3 lightDirection)
{
    float3 reflection = reflect(lookDirection, normal);
    return pow(max(dot(reflection, -lightDirection), 0), 16);
}

// inverse shadow amount (lightness)
float shadow(sampler tex, float4 position)
{
    float4 shadowUV = position;
    shadowUV /= shadowUV.w;
    shadowUV = (shadowUV * .5 + .5);
    shadowUV.y = 1 - shadowUV.y;
    
    float shadowDepth = tex2D(tex, shadowUV.xy).r;
    float depth = position.z / position.w;
    return smoothstep(.002, .00001, depth - shadowDepth); // shadow bias
}

float4 Diffuse_PixelShader(Diffuse_VSOut input) : COLOR
{
    float4 alberto = tex2D(Alberto, input.UV);
    float3 ao = pow(tex2D(AmbientOcclusion, input.UV), 4);
    
    float3 lightDirection = normalize(LightDirection);
    float3 normal = normalize(input.Normal); // in world space
    float3 lookDirection = normalize(input.WorldPosition - CameraPosition);
    
    float3 color = (
            (light(lookDirection, normal, lightDirection) + diffuse(normal, lightDirection))
            * shadow(ShadowSampler, input.ShadowPosition)
            + ambient()
        ) * alberto * ao;
    
    return float4(color, 1);
}

technique Diffuse
{
    pass Pass1
    {
        VertexShader = compile vs_5_0 Diffuse_VertexShader();
        PixelShader = compile ps_5_0 Diffuse_PixelShader();
    }
}
