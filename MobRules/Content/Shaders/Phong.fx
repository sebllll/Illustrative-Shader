float4x4 wvp;
float4x4 world;
float4   light;
float4   colour;
float    intensity;
Texture  tex;

sampler textureSampler = sampler_state
{
  Texture   = (tex);
  Minfilter = LINEAR;
  Magfilter = LINEAR;
  Mipfilter = LINEAR;
};

struct VSInput
{
    float4 Position : POSITION0;
    float3 Normal   : NORMAL0;
    float2  UV      : TEXCOORD0;
};

struct VSOutput
{
    float4 Position  : POSITION;  // Position for view and camera
    float3 XPosition : TEXCOORD0; // Transformed Position
    float2 Tex       : TEXCOORD1; // UV data
    float3 Normal    : TEXCOORD2; // Normalized normal in world space
    float4 Amb       : COLOR0;
};

// Get the Background Light
float4 GetAmbientLight()
{
  float4 colour = {1.0f, 1.0f, 1.0f, 1.0f};
  float intensity = 0.1f;
  return colour * intensity;
}

float4 GetSpecularLight(VSOutput input)
{
  // Light and material properties
  float4 intensity = 0.2f;
  float4 col = {1.0f, 1.0f, 0.0f, 1.0f};
  
  float3 unitNorm = input.Normal; // N
  float3 unitLightDir = normalize(light - input.XPosition); // L
  float cosA = dot(unitNorm, unitLightDir); // (N.L)
  
  // R = 2 * (N.L) * N - L
  float3 reflect = normalize(2 * cosA * unitNorm - unitLightDir);
  
  // (R.V) ^ n - Specular Reflection 
  float specRef = pow(dot(reflect, unitLightDir), 2);
  
  float4 spec = col * intensity * specRef;
  
  return spec;
}

float4 GetPointLightDiffuse(VSOutput input)
{
  float3 unitLightDir = normalize(light - input.XPosition); // Unit direction of L
  float diffuse = dot(unitLightDir, input.Normal); // Brightest angle between L and N = 0
  return diffuse * intensity * colour;
}

VSOutput VertexShaderFunction(VSInput input)
{
    VSOutput output;

    output.Position  = mul(input.Position, wvp);
    output.XPosition = mul(input.Position, world);
    output.Normal    = normalize(mul(input.Normal, (float3x3)world));
    output.Tex       = input.UV;
    output.Amb       = GetAmbientLight();

    return output;
}

float4 PixelShaderFunction(VSOutput input) : COLOR0
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
