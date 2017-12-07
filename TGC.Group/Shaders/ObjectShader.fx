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
float windNormalX;
float windNormalZ;
float windIntencidad;

/**************************************************************************************/
/* RenderScene */
/**************************************************************************************/
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

//Pixel Shader default
float4 ps_main(float2 Texcoord: TEXCOORD0, float4 Color : COLOR0) : COLOR0{
	float4 fvBaseColor = tex2D(diffuseMap, Texcoord);
	
	return fvBaseColor;
}

//Vertex Shader default
VS_OUTPUT vs_default(VS_INPUT Input){
	VS_OUTPUT Output;

	Output.Position = mul(Input.Position, matWorldViewProj);
	Output.Texcoord = Input.Texcoord;
	Output.Color = Input.Color;

	return(Output);
}

//Vertex Shader wind
VS_OUTPUT vs_wind(VS_INPUT Input){
	VS_OUTPUT Output;
    if (windNormalX > windNormalZ) {
        windNormalZ = 0;
    } else {
        windNormalX = 0;
    }
    float potWind = windIntencidad*30;

    Input.Position.x += windNormalX * Input.Position.y * windIntencidad + (windNormalX*(sin(time * potWind) * Input.Position.y)/100);
    Input.Position.z += windNormalZ * Input.Position.y * windIntencidad + (windNormalZ*(cos(time * potWind) * Input.Position.y)/100);

    Output.Position = mul(Input.Position, matWorldViewProj);
    Output.Texcoord = Input.Texcoord;
    Output.Color = Input.Color;

    return (Output);
}

//Pixel Shader wind
float4 ps_main_wind(float2 Texcoord: TEXCOORD0, float4 Color : COLOR0) : COLOR0{
	float4 fvBaseColor = tex2D(diffuseMap, Texcoord);
	return fvBaseColor;
}

VS_OUTPUT vs_agua(VS_INPUT Input) {
    VS_OUTPUT Output;

    float periodo = time/8;
    float amplitud = 0.05;

    Input.Position.y += sin(time / 2) * 0.1;
    Input.Texcoord.y += sin(periodo) * amplitud;
    Input.Texcoord.x += cos(periodo) * amplitud;

    Output.Position = mul(Input.Position, matWorldViewProj);
    Output.Texcoord = Input.Texcoord;
    Output.Color = Input.Color;

    return (Output);
}

float4 ps_main_agua(float2 Texcoord: TEXCOORD0, float4 Color : COLOR0) : COLOR0{
	float4 fvBaseColor = tex2D(diffuseMap, Texcoord);
    return fvBaseColor;
}

technique Default{
	pass Pass_0 {
		VertexShader = compile vs_3_0 vs_default();
		PixelShader = compile ps_3_0 ps_main();
	}
}

technique Wind {
    pass Pass_0 {
        VertexShader = compile vs_3_0 vs_wind();
        PixelShader = compile ps_3_0 ps_main_wind();
    }
}

technique Agua {
    pass P0 {
        VertexShader = compile vs_3_0 vs_agua();
        PixelShader = compile ps_3_0 ps_main_agua();
    }
}