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

float distCamMesh;

float StartFogDistance;
float EndFogDistance;
float Density;

/* RenderScene */
struct VS_INPUT_VERTEX {
    float4 Position : POSITION0;
	float3 Texture : TEXCOORD0;
};


//Output del Vertex Shader
struct VS_OUTPUT_VERTEX {
    float4 Position : POSITION0;
	float2 Texture:    TEXCOORD0;
	float1 Fog:     FOG;
};
                               
//Input del Vertex Shader
struct VS_INPUT {
    float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT {
    float4 Position :        POSITION0;
	float2 Texcoord :        TEXCOORD0;
	float4 Color :			COLOR0;
};

float4 ps_main_default(float2 Texcoord: TEXCOORD0, float4 Color : COLOR0) : COLOR0{
	return tex2D(diffuseMap, Texcoord);
}

//Pixel Shader
float4 ps_main_fog(VS_OUTPUT_VERTEX input) : COLOR0{
	// Obtener el texel de textura
	// diffuseMap es el sampler, Texcoord son las coordenadas interpoladas
	float4 fvBaseColor = tex2D(diffuseMap, input.Texture);
    // combino fog y textura
    float4 fogFactor = float4(input.Fog, input.Fog, input.Fog, input.Fog);

    fvBaseColor.a += (1.0 - fogFactor);
    return fogFactor* fvBaseColor; 
}

VS_OUTPUT vs_default(VS_INPUT Input){
	VS_OUTPUT Output;

	Output.Position = mul(Input.Position, matWorldViewProj);
	Output.Texcoord = Input.Texcoord;
	Output.Color = Input.Color;

	return Output;
}

VS_OUTPUT_VERTEX vs_wind_fog(VS_INPUT_VERTEX input) {
    VS_OUTPUT_VERTEX output;

    if (windNormalX > windNormalZ) {
        windNormalZ = 0;
    } else {
        windNormalX = 0;
    }
    float potWind = windIntencidad * 30;

    input.Position.x += windNormalX * input.Position.y * windIntencidad + (windNormalX * (sin(time * potWind) * input.Position.y) / 100);
    input.Position.z += windNormalZ * input.Position.y * windIntencidad + (windNormalZ * (cos(time * potWind) * input.Position.y) / 100);

    output.Position = mul(input.Position, matWorldViewProj);
    output.Texture = input.Texture;

    output.Fog = saturate((EndFogDistance - distCamMesh) / (EndFogDistance - StartFogDistance));
    return output;
}

VS_OUTPUT vs_agua(VS_INPUT Input) {
    VS_OUTPUT Output;

    float periodo = time / 8;
    float amplitud = 0.05;

    Input.Position.y += sin(time / 2) * 0.1;
    Input.Texcoord.y += sin(periodo) * amplitud;
    Input.Texcoord.x += cos(periodo) * amplitud;

    Output.Position = mul(Input.Position, matWorldViewProj);
    Output.Texcoord = Input.Texcoord;
    Output.Color = Input.Color;

    return Output;
}

VS_OUTPUT_VERTEX vs_main_fog(VS_INPUT_VERTEX input) {
    VS_OUTPUT_VERTEX output;

    //Proyectar posicion
    output.Position = mul(input.Position, matWorldViewProj);
    output.Texture = input.Texture;
    output.Fog = saturate((EndFogDistance - distCamMesh) / (EndFogDistance - StartFogDistance));
    return output;
}

// ------------------------------------------------------------------
technique Default {
    pass Pass_0 {
        VertexShader = compile vs_3_0 vs_default();
        PixelShader = compile ps_3_0 ps_main_default();
    }
}

technique Fog {
    pass Pass_0 {
        VertexShader = compile vs_3_0 vs_main_fog();
        PixelShader = compile ps_3_0 ps_main_fog();
    }
}

technique Wind {
    pass Pass_0 {
        VertexShader = compile vs_3_0 vs_wind_fog();
        PixelShader = compile ps_3_0 ps_main_fog();
    }
}

technique Agua {
    pass P0 {
        VertexShader = compile vs_3_0 vs_agua();
        PixelShader = compile ps_3_0 ps_main_default();
    }
}
