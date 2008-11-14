float4x4 wvp;
float4x4 world;
float4   light;
float4   colour;
float    intensity;
Texture  tex;

sampler textureSampler = sampler_state
{
  Texture = (tex);
  Minfilter = LINEAR;
  Magfilter = LINEAR;
  Mipfilter = LINEAR;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal   : NORMAL0;
    float  UV       : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 XPos     : TEXCOORD0; // Transformed Position
    float2 Tex      : TEXCOORD1; // UV data
    float3 Normal   : TEXCOORD2; // Normalized normal
    float4 Amb      : COLOR0;    // 
};

//struct PixelShaderOutput
//{
//    float4 colour : COLOR; 
//}

// Get the Background Light
float4 GetAmbientLight()
{
  float4 colour = 1.0f;
  float intensity = 0.1f;
  return colour * intensity;
}

float4 GetSpecularLight(VertexShaderOutput input)
{
  // Light and material properties
  float4 intensity = 0.2f;
  float4 colour = (1.0f, 1.0f, 0.0f, 1.0f);
  
  float3 unitNorm = input.Normal;
  float3 unitLightDir = normalize(light - input.XPos);
  //float3 unitLightDir = normalize(lightDir);
  
  // (N.L)
  float cosA = dot(unitNorm, unitLightDir);
  
  // R = 2 * (N.L) * N - L
  float3 reflect = normalize(2 * cosA * unitNorm - unitLightDir);
  
  // (R.V) ^ n - Specular Reflection 
  float specMag = pow(dot(reflect, unitLightDir), 2);
  
  float4 spec = colour * intensity * specMag;
  
  return spec;
}

float4 GetPointLightDiffuse(VertexShaderOutput input)
{
  float3 unitLightDir = normalize(light - input.XPos);
  float diffuse = dot(light, input.Normal);
  return diffuse * intensity * colour;
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(input.Position, wvp);
    output.XPos     = mul(input.Position, world);
    output.Normal   = normalize(mul(input.Normal, (float3x3)world));
    output.Tex      = input.UV;
    output.Amb      = GetAmbientLight();

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 diffuse  = GetPointLightDiffuse(input);
    float4 specular = GetSpecularLight(input);
    return tex2D(textureSampler, input.Tex) * (input.Amb + specular + diffuse);
}

technique Technique1
{
    pass Pass1
    {
        sampler[0] = (textureSampler);
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
