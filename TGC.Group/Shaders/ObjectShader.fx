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
//wind
float time = 0;
float windNormalX;
float windNormalZ;
float windIntencidad;
//fog
float4 ColorFog;
float distCamMesh;
float StartFogDistance;
float EndFogDistance;
float Density;
//Light
//Material del mesh
float3 materialEmissiveColor; //Color RGB
float3 materialAmbientColor; //Color RGB
float4 materialDiffuseColor; //Color ARGB (tiene canal Alpha)
//Parametros de la Luz
float3 lightColor; //Color RGB de la luz
float4 lightPosition; //Posicion de la luz
float4 eyePosition; //Posicion de la camara
float lightIntensity; //Intensidad de la luz
float lightAttenuation; //Factor de atenuacion de la luz
//Intensidad de efecto Bump
float bumpiness;
const float3 BUMP_SMOOTH = { 0.5f, 0.5f, 1.0f };

float reflection;

texture texNormalMap;
sampler2D normalMap = sampler_state
{
	Texture = (texNormalMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

struct VS_INPUT3 {
    float4 Position : POSITION0;
	float3 Normal :   NORMAL0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
	float3 Tangent : TANGENT0;
	float3 Binormal : BINORMAL0;
};

struct VS_OUTPUT3 {
    float4 Color : COLOR;
    float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
	float3 WorldPosition : TEXCOORD1;
	float3 WorldNormal : TEXCOORD2;
	float3 WorldTangent	: TEXCOORD3;
	float3 WorldBinormal : TEXCOORD4;
	float3 LightVec	: TEXCOORD5;
	float3 HalfAngleVec	: TEXCOORD6;
};

//Input del Pixel Shader
struct PS_INPUT3 {
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float1 Fog:     FOG;
	float3 WorldPosition : TEXCOORD1;
	float3 WorldNormal : TEXCOORD2;
	float3 WorldTangent	: TEXCOORD3;
	float3 WorldBinormal : TEXCOORD4;
	float3 LightVec	: TEXCOORD5;
	float3 HalfAngleVec	: TEXCOORD6;
};

//Pixel Shader
float4 ps_fog_light(PS_INPUT3 input) : COLOR0{
    //float4 fogFactor = float4(input.Fog, input.Fog, input.Fog, input.Fog);
    //finalColor.a += (1.0 - fogFactor);

    float4 finalFogColor = tex2D(diffuseMap, input.Texcoord);
    if (distCamMesh > StartFogDistance)
        if (distCamMesh > EndFogDistance){
            finalFogColor.a = 0;
        }else{
            float1 total = EndFogDistance - StartFogDistance;
            float1 delta = distCamMesh - StartFogDistance;
            finalFogColor.a = 1 - (delta / total);
        }
	//Normalizar vectores
	float3 Nn = normalize(input.WorldNormal);
    float3 Ln = normalize(input.LightVec);
    float3 Tn = normalize(input.WorldTangent);
    float3 Bn = normalize(input.WorldBinormal);

    //Calcular intensidad de luz, con atenuacion por distancia
    float distAtten = length(lightPosition.xyz - input.WorldPosition) * lightAttenuation;
    float intensity = lightIntensity / distAtten; //Dividimos intensidad sobre distancia (lo hacemos lineal pero tambien podria ser i/d^2)

    //Obtener texel de la textura
    float4 texelColor = tex2D(diffuseMap, input.Texcoord);

    //Obtener normal de normalMap y ajustar rango de [0, 1] a [-1, 1]
    float3 bumpNormal = tex2D(normalMap, input.Texcoord).rgb;
    bumpNormal = (bumpNormal* 2.0f) - 1.0f;

	//Suavizar con bumpiness
	bumpNormal = lerp(BUMP_SMOOTH, bumpNormal, bumpiness);

    //Pasar de Tangent-Space a World-Space
    bumpNormal = Nn + bumpNormal.x* Tn + bumpNormal.y* Bn;
    bumpNormal = normalize(bumpNormal);

    //Componente Ambient
    float3 ambientLight = intensity * lightColor * materialAmbientColor;

    //Componente Diffuse: N dot L, usando normal de NormalMap
    float3 n_dot_l = dot(bumpNormal, Ln);
    float3 diffuseLight = intensity * lightColor * materialDiffuseColor.rgb * max(0.0, n_dot_l); //Controlamos que no de negativo

    /* Color final: modular (Emissive + Ambient + Diffuse) por el color de la textura, y luego sumar Specular.El color Alpha sale del diffuse material */
    float4 finalColor = float4((materialEmissiveColor + ambientLight + diffuseLight) * texelColor, materialDiffuseColor.a);

	return finalFogColor /** finalColor*/  * texelColor;
}

//Vertex Shader
VS_OUTPUT3 vs_light(VS_INPUT3 input) {
    VS_OUTPUT3 output;
    
    output.Position = mul(input.Position, matWorldViewProj);
    output.Texcoord = input.Texcoord;
    output.Color = input.Color;

    //Posicion pasada a World-Space
    output.WorldPosition = mul(input.Position, matWorld).xyz;
    //Pasar normal, tangent y binormal a World-Space
    output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;
    output.WorldTangent = mul(input.Tangent, matInverseTransposeWorld).xyz;
    output.WorldBinormal = mul(input.Binormal, matInverseTransposeWorld).xyz;
    //LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
    output.LightVec = lightPosition.xyz - output.WorldPosition;
    //ViewVec (V): vector que va desde el vertice hacia la camara.
    float3 viewVector = eyePosition.xyz - output.WorldPosition;
    //HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
    output.HalfAngleVec = viewVector + output.LightVec;

    return output;
}

VS_OUTPUT3 vs_wind_Light(VS_INPUT3 input) {
    VS_OUTPUT3 output;
    
    if (windNormalX > windNormalZ) {
        windNormalZ = 0;
    } else {
        windNormalX = 0;
    }
    float potWind = windIntencidad * 30;

    input.Position.x += windNormalX * input.Position.y * windIntencidad + (windNormalX * (sin(time * potWind) * input.Position.y) / 100);
    input.Position.z += windNormalZ * input.Position.y * windIntencidad + (windNormalZ * (cos(time * potWind) * input.Position.y) / 100);

    output.Position = mul(input.Position, matWorldViewProj);
    output.Texcoord = input.Texcoord;
    output.Color = input.Color;

    //Posicion pasada a World-Space
    output.WorldPosition = mul(input.Position, matWorld).xyz;
    //Pasar normal, tangent y binormal a World-Space
    output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;
    output.WorldTangent = mul(input.Tangent, matInverseTransposeWorld).xyz;
    output.WorldBinormal = mul(input.Binormal, matInverseTransposeWorld).xyz;
    //LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
    output.LightVec = lightPosition.xyz - output.WorldPosition;
    //ViewVec (V): vector que va desde el vertice hacia la camara.
    float3 viewVector = eyePosition.xyz - output.WorldPosition;
    //HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
    output.HalfAngleVec = viewVector + output.LightVec;
    
    return output;
}


//Vertex Shader
VS_OUTPUT3 vs_agua_light(VS_INPUT3 input) {
    VS_OUTPUT3 output;
    float periodo = time / 8;
    float amplitud = 0.05;

    //Posicion pasada a World-Space
    output.WorldPosition = mul(input.Position, matWorld).xyz;

    //Pasar normal, tangent y binormal a World-Space
    output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;
    output.WorldTangent = mul(input.Tangent, matInverseTransposeWorld).xyz;
    output.WorldBinormal = mul(input.Binormal, matInverseTransposeWorld).xyz;

    //LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
    output.LightVec = lightPosition.xyz - output.WorldPosition;

    //ViewVec (V): vector que va desde el vertice hacia la camara.
    float3 viewVector = eyePosition.xyz - output.WorldPosition;

    //HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
    output.HalfAngleVec = viewVector + output.LightVec;
    
    input.Position.y += sin(time / 2) * 0.1;
    input.Texcoord.y += sin(periodo) * amplitud;
    input.Texcoord.x += cos(periodo) * amplitud;

    output.Position = mul(input.Position, matWorldViewProj);
    output.Texcoord = input.Texcoord;
    output.Color = input.Color;

    return output;
}

technique AguaLight {
    pass P0 {
        VertexShader = compile vs_3_0 vs_agua_light();
        PixelShader = compile ps_3_0 ps_fog_light();
    }
}

technique FogLight {
    pass Pass_0 {
        VertexShader = compile vs_3_0 vs_light();
        PixelShader = compile ps_3_0 ps_fog_light();
    }
}

technique FogWindLight {
    pass Pass_0 {
        VertexShader = compile vs_3_0 vs_wind_Light();
        PixelShader = compile ps_3_0 ps_fog_light();
    }
}