float4x4 World;
float4x4 View;
float4x4 Projection;
float3   Camera;
float4   Light;
float4   LightColour;
float    Intensity;

// Texture Options
texture  Texture, WarpTexture, SpecTexture, SpecMaskTexture;
bool TextureEnabled;

sampler textureSampler = sampler_state
{
  Texture   = (Texture);
  Minfilter = LINEAR;
  Magfilter = LINEAR;
  Mipfilter = LINEAR;
};

sampler warpTextureSampler = sampler_state
{
  Texture = (WarpTexture);
  Minfilter = LINEAR;
  Magfilter = LINEAR;
  Mipfilter = LINEAR;
};

sampler specTextureSampler = sampler_state
{
  Texture = (SpecTexture);
  Minfilter = LINEAR;
  Magfilter = LINEAR;
  Mipfilter = LINEAR;
};

sampler specMaskTextureSampler = sampler_state
{
  Texture = (SpecMaskTexture);
  Minfilter = LINEAR;
  Magfilter = LINEAR;
  Mipfilter = LINEAR;
};

struct VSInput
{
  float4 Position : POSITION0;
  float3 Normal   : NORMAL0;
  float2 UV       : TEXCOORD0;
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
  float intensity = 0.6f;
  return colour * intensity;
}

float4 W(float4 input)
{
  return tex2D(warpTextureSampler, input);
}

VSOutput VertexShaderFunction(VSInput input)
{
    VSOutput output;
    
    output.Position  = mul(mul(mul(input.Position, World), View), Projection);
    output.XPosition = mul(input.Position, World);
    output.Normal    = normalize(mul(input.Normal, (float3x3)World));
    output.Tex       = input.UV;
    output.Amb       = GetAmbientLight();

    return output;
}

float4 PixelShaderFunction(VSOutput input) : COLOR0
{
    // View Independent Terms
    float4 kd = tex2D(textureSampler, input.Tex);
    
    float4 lightColour = LightColour;
    float3 unitView = normalize(Camera - input.XPosition);
    float3 unitNorm = input.Normal; // N
    float3 unitLightDir = normalize(Light - input.XPosition); // L
    
    float a = 0.5f; // From Gooch
    float b = 0.5f; // From Gooch
    int y   = 1;    // From p19-mitchell

    float4 viewIndependent = kd * (input.Amb + lightColour * W( pow((a * dot(unitNorm, unitLightDir) + b), y)));
    
    // View Dependent Terms
    float cosA = dot(unitNorm, unitLightDir); // (N.L)
    float3 r = normalize(2 * cosA * unitNorm - unitLightDir); // R = 2 * (N.L) * N - L
    float3 up = {0.0f, 1.0f, 0.0f};
    
    float4 ks = tex2D(specMaskTextureSampler, input.Tex); // Specular
    float kSpecExp = tex2D(specTextureSampler, input.Tex)[0];
    //float4 kr = tex2D(rimTextureSampler, input.Tex); // Rim Highlight mask (Optional for Problem Cases)
    
    float cosB = dot(unitView, r);
    
    // Rim Highlight
    float4 fs = pow(1.4 - dot(unitNorm, unitView), 4);
    float4 fr = pow(1 - dot(unitNorm, unitView), 4);
    
    float4 viewDependent = lightColour * ks * max(fs * pow(cosB, kSpecExp), fr * pow(cosB, 2));
    viewDependent += dot(unitNorm, up) * fr * (input.Amb, 0.6f);
    
    return viewIndependent + viewDependent;
}

technique Technique1
{
    pass Pass1
    {
        sampler[0] = (textureSampler);
        sampler[1] = (warpTextureSampler);
        sampler[2] = (specTextureSampler);
        sampler[3] = (specMaskTextureSampler);
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
