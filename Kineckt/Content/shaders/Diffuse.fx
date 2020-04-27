// You can only upload uniforms that you also use in code

matrix World;
matrix View;
matrix Projection;
matrix WorldInverse;
matrix ViewInverse;
matrix ProjectionInverse;

sampler Alberto;
sampler AmbientOcclusion;
sampler NormalMap;

texture Shadow;
sampler ShadowSampler = sampler_state
{
    Texture = <Shadow>;
    AddressU = Clamp;
    AddressV = Clamp;
};
matrix ShadowProjection;

float3 LightDirection;
float3 LookDirection;

struct Diffuse_VSOut
{
    float4 Position : POSITION;
    float2 UV :TEXCOORD0;
    float4 ShadowPosition : TEXCOORD2;
    float3 Normal : NORMAL;
};

//  CREATE SHADOW MAP
Diffuse_VSOut Diffuse_VertexShader(float4 Position : SV_POSITION, float2 UV : TEXCOORD0, float3 Normal : NORMAL)
{
    Diffuse_VSOut Out;
    Out.Position = mul(Position, mul(World, mul(View, Projection)));
    Out.ShadowPosition = mul(Position, ShadowProjection);
    Out.UV = UV;
    Out.Normal = mul(float4(Normal,0) , World);
    
    return Out;
}

float4 Diffuse_PixelShader(Diffuse_VSOut input) : COLOR
{
    float4 alberto = tex2D(Alberto, input.UV);
    float3 normalMap = normalize(tex2D(NormalMap, input.UV).rgb * 2 - 1);
    float4 ao = tex2D(AmbientOcclusion, input.UV);
    ao = ao * ao * ao * ao;
    
    float4 shadowUV = input.ShadowPosition;
    shadowUV /= shadowUV.w;
    shadowUV = (shadowUV * .5 +.5);
    shadowUV.y = 1 - shadowUV.y;
    float shadowDepth = tex2D(ShadowSampler, shadowUV.xy).r;
    float depth = input.ShadowPosition.z / input.ShadowPosition.w;
    float shadow = smoothstep(.002, .00001, depth - shadowDepth);
    float3 shadowColor = (float3(shadow, shadow, shadow) * .5) + .5;
    
    float3 lightDir = normalize(LightDirection);
    float3 normal = input.Normal; // in world space
    
    if(length(normalMap) > 0) {
        normal = (input.Normal + normalMap * .01);
    }
    normal = normalize(normal);
    
    float3 reflection = reflect(LookDirection, normal);
    float light = pow(max(dot(reflection, -lightDir), 0), 32);
    float diffuse = max(dot(-lightDir, normal), 0);
    float ambient = .3;
    
    float3 color = (light + diffuse + ambient) * alberto * shadowColor * ao.rgb;
    
    // color.rgb = lerp(color.rgb, normal, 0.99);
    // color.rgb = lerp(color.rgb, light, 0.99999);
    
    return float4(color, 1);
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
