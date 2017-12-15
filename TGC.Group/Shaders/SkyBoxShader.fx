//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matView; //Matriz de View
float4x4 matProjection; //Matriz de projection
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};
    
float factor;

struct VS_INPUT {
    float4 Position : POSITION0;
	float3 Texture : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT {
    float4 Position : POSITION0;
	float2 Texture:    TEXCOORD0;
};

//Pixel Shader
float4 ps_alpha(VS_OUTPUT input) : COLOR0{
	float4 fvBaseColor = tex2D(diffuseMap, input.Texture);
    fvBaseColor.a = (1 - factor);

    return fvBaseColor; 
}


VS_OUTPUT vs_alpha(VS_INPUT input) {
    VS_OUTPUT output;
    output.Position = mul(input.Position, matWorldViewProj);
    output.Texture = input.Texture;
    return output;
}

technique Alpha {
    pass Pass_0 {
        VertexShader = compile vs_3_0 vs_alpha();
        PixelShader = compile ps_3_0 ps_alpha();
    }
}