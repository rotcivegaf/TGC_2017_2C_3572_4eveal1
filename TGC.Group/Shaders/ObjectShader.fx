/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

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

float time = 0;
float luz = 0;
float2 wind ;
float meshHeight;
float3 meshPosition;
float bendFactor = 0;

/**************************************************************************************/
/* RenderScene */
/**************************************************************************************/

// This bends the entire plant in the direction of the wind.
// vPos:		The world position of the plant *relative* to the base of the plant.
//			(That means we assume the base is at (0, 0, 0). Ensure this before calling this function).
// vWind:		The current direction and strength of the wind.
// fBendScale:	How much this plant is affected by the wind.
void ApplyMainBending(inout float4 vPos, float2 vWind, float fBendScale){
	// Calculate the length from the ground, since we'll need it.
	float fLength = length(vPos);
	// Bend factor - Wind variation is done on the CPU.
	float fBF = vPos.y * fBendScale;
	// Smooth bending factor and increase its nearby height limit.
	fBF += 1.0;
	fBF *= fBF;
	fBF = fBF * fBF - fBF;
	// Displace position
	float4 vNewPos = vPos;
	vNewPos.xz += fBF;

	// Rescale - this keeps the plant parts from "stretching" by shortening the y (height) while
	// they move about the xz.

	vNewPos.xz += (fBF *time);
	vPos.xyz = normalize(vNewPos.xyz)* fLength;
}

//Input del Vertex Shader
struct VS_INPUT{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT{
	float4 Position :        POSITION0;
	float2 Texcoord :        TEXCOORD0;
	float4 Color :			COLOR0;
};

//Vertex Shader default
VS_OUTPUT vs_default(VS_INPUT Input){
	VS_OUTPUT Output;
	//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);
	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;
	//Propago el color x vertice
	Output.Color = Input.Color;

	return(Output);
}

//Vertex Shader wind
VS_OUTPUT vs_wind(VS_INPUT Input){
	VS_OUTPUT Output;

	ApplyMainBending(Input.Position, wind, bendFactor);
	//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);
	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;
	//Propago el color x vertice
	Output.Color = Input.Color;

	return(Output);
}

//Pixel Shader default
float4 ps_main(float2 Texcoord: TEXCOORD0, float4 Color : COLOR0) : COLOR0{
	float4 fvBaseColor = tex2D(diffuseMap, Texcoord);
	
	return fvBaseColor;
}

//Pixel Shader wind
float4 ps_main_wind(float2 Texcoord: TEXCOORD0, float4 Color : COLOR0) : COLOR0{
	// Obtener el texel de textura
	// diffuseMap es el sampler, Texcoord son las coordenadas interpoladas
	float4 fvBaseColor = tex2D(diffuseMap, Texcoord);
	// combino color y textura
	//float4 result = fvBaseColor * luz;
	//result.a = fvBaseColor.a;
	//return result;
	return fvBaseColor;
}

technique Default{
	pass Pass_0 {
		VertexShader = compile vs_3_0 vs_default();
		PixelShader = compile ps_3_0 ps_main();
	}
}

technique Wind{
	pass Pass_0 {
		VertexShader = compile vs_3_0 vs_wind();
		PixelShader = compile ps_3_0 ps_main_wind();
	}
}