//------------------------------------global inputs----------------------
float4x4 xLightsWorldViewProjection;

float4x4 xWorld;
float3 xLightPos;
float xLightPower [2];
float xAmbient;
float4x4 xViewProjection;

float3 xLamppostPos[4];

float xLightAttenuation;
float xLightFallOff;
float xCamRads [4];
float xCamLtTimesGlow [4];
float4 xLtscrPositions [4];

//--------------------------------------Texture Samplers-----------------------------------
Texture xTexture; 
sampler TextureSampler = sampler_state
	{
		texture = <xTexture>;
		magfilter = LINEAR;
		minfilter = LINEAR;
		mipfilter = LINEAR;
		AddressU = mirror;
		AddressV = mirror;
	};

Texture xShadowMap;
sampler ShadowMapSampler = sampler_state 
	{
		texture = <xShadowMap>;
		magfilter = LINEAR;
		minfilter = LINEAR;
		mipfilter = LINEAR;
		AddressU = clamp;
		AddressV = clamp;
	};

Texture xCarLightTexture;
sampler CarLightSampler = sampler_state 
	{ 
		texture = <xCarLightTexture>;
		magfilter = LINEAR;
		minfilter = LINEAR;
		mipfilter = LINEAR;
		AddressU = clamp;
		AddressV = clamp;
	};
	
	
//--------------------------------- Subroutines------------------------
float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
	{
		float3 lightDir = normalize(pos3D - lightPos);
		return dot(-lightDir, normal);
	}


//---------------------Techniques------------------
	//-----------------------------Technique: SimpleTexture------------------------
	struct VertexToPixel
			{
				float4 Position : POSITION;
				float2 TexCoords : TEXCOORD0;
				float3 Normal : TEXCOORD1;
				float3 Position3D : TEXCOORD2;
			};

	struct PixelToFrame
			{
				float4 Color : COLOR0;
			};

	VertexToPixel STVertexShader( float4 inPos : POSITION0, float3 inNormal: NORMAL0, float2 inTexCoords : TEXCOORD0)
			{
				VertexToPixel Output = (VertexToPixel)0;

				float4x4 preWorldViewProjection = mul (xWorld, xViewProjection);
				Output.Position = mul(inPos, preWorldViewProjection);
				Output.TexCoords = inTexCoords;
				Output.Normal = normalize(mul(inNormal, (float3x3)xWorld));
				Output.Position3D = mul(inPos, xWorld);
				
				return Output;
			}


	PixelToFrame STPixelShader(VertexToPixel PSIn)
		{
			PixelToFrame Output = (PixelToFrame)0;
			
			float diffuseLightingFactor = DotProduct(xLightPos, PSIn.Position3D, PSIn.Normal);
			diffuseLightingFactor = saturate(diffuseLightingFactor);
			diffuseLightingFactor *= xLightPower [0];

			PSIn.TexCoords.y--;

			float4 baseColor = tex2D(TextureSampler, PSIn.TexCoords);
			Output.Color = baseColor*(diffuseLightingFactor + xAmbient);
			
			return Output;
		}

	technique SimpleTexture
		{
			pass Pass0
				{
					VertexShader = compile vs_2_0 STVertexShader();
					PixelShader = compile ps_2_0 STPixelShader();
				}
		}

		//------- Technique: SimpleNormal --------
		struct SNVertexToPixel
			{
				float4 Position : POSITION;
				float3 Normal : TEXCOORD0;
				float3 Position3D : TEXCOORD1;
				float4 Pos2DAsSeenByLight : TEXCOORD2;
			};

	struct SNPixelToFrame
		{
			float4 Color : COLOR0;
		};
		
	SNVertexToPixel SNVertexShader(float4 inPos : POSITION, float3 inNormal: NORMAL)
		{
			SNVertexToPixel Output = (SNVertexToPixel)0;
		
			float4x4 preWorldViewProjection = mul (xWorld, xViewProjection);
			Output.Position = mul(inPos, preWorldViewProjection);
			Output.Normal = normalize(mul(inNormal, (float3x3)xWorld));
			Output.Position3D = mul(inPos, xWorld);
			Output.Pos2DAsSeenByLight = mul(inPos, xLightsWorldViewProjection);
			
			return Output;
		}

	SNPixelToFrame SNPixelShader(SNVertexToPixel PSIn)
		{
			SNPixelToFrame Output = (SNPixelToFrame)0;
			float2 ProjectedTexCoords;
			ProjectedTexCoords[0] = PSIn.Pos2DAsSeenByLight.x/PSIn.Pos2DAsSeenByLight.w/2.0f +0.5f;
			ProjectedTexCoords[1] = -PSIn.Pos2DAsSeenByLight.y/PSIn.Pos2DAsSeenByLight.w/2.0f +0.5f;

			float4 baseColor = float4(.1, .1, .1, 1); // pick a dark gray color
			float diffuseLightingFactor = 0;

			if ((saturate(ProjectedTexCoords).x == ProjectedTexCoords.x) && (saturate(ProjectedTexCoords).y == ProjectedTexCoords.y))
				{
					float depthStoredInShadowMap = tex2D(ShadowMapSampler, ProjectedTexCoords).r;
					float realDistance = PSIn.Pos2DAsSeenByLight.z/PSIn.Pos2DAsSeenByLight.w;
					diffuseLightingFactor = DotProduct(xLightPos, PSIn.Position3D, PSIn.Normal);
					diffuseLightingFactor = saturate(diffuseLightingFactor);
					if ((realDistance - 0.01f) <= depthStoredInShadowMap)
						{
							diffuseLightingFactor *= xLightPower[0];
						}
						else diffuseLightingFactor *= xLightPower [1];
				}

	Output.Color = baseColor*(diffuseLightingFactor + xAmbient);

	// Add some point emissive lighting for the lamps
	// No reason not to do the computation in world space

	float dist0 = distance(PSIn.Position3D, xLamppostPos[0]);
	float dist1 = distance(PSIn.Position3D, xLamppostPos[1]);

	//We want a different level of emissive light for each lamp
	if (dist0 < 1)
		{
			Output.Color.rgb += (xCamLtTimesGlow [0] * (1-dist0));
		}
	if (dist1 < 1)
		{
			Output.Color.rgb += (xCamLtTimesGlow [1] * (1-dist1));
		}

			
			return Output;
		}

	technique SimpleNormal
		{
			pass Pass0
				{
					VertexShader = compile vs_2_0 SNVertexShader();
					PixelShader = compile ps_2_0 SNPixelShader();
				}
		}


	//------- Technique: ShadowMap --------
	struct SMapVertexToPixel
		{
			float4 Position : POSITION;
			float4 Position2D : TEXCOORD0;
		};
	struct SMapPixelToFrame
		{
			float4 Color : COLOR0;
		};

	SMapVertexToPixel ShadowMapVertexShader( float4 inPos : POSITION)
		{
			SMapVertexToPixel Output = (SMapVertexToPixel)0;
			Output.Position = mul(inPos, xLightsWorldViewProjection);
			Output.Position2D = Output.Position;
			
			return Output;
		}

	SMapPixelToFrame ShadowMapPixelShader(SMapVertexToPixel PSIn)
		{
			SMapPixelToFrame Output = (SMapPixelToFrame)0;
			Output.Color = PSIn.Position2D.z/PSIn.Position2D.w;
			return Output;
		}
	
	technique ShadowMap
		{
			pass Pass0
		{

			VertexShader = compile vs_2_0 ShadowMapVertexShader();
			PixelShader = compile ps_2_0 ShadowMapPixelShader();
		}
	}


	//------- Technique: ShadowedScene --------
	struct SSceneVertexToPixel
		{
			float4 Position : POSITION;
			float4 Pos2DAsSeenByLight : TEXCOORD0;
			float2 TexCoords : TEXCOORD1;
			float3 Normal : TEXCOORD2;
			float4 Position3D : TEXCOORD3;
		};
	struct SScenePixelToFrame
		{
			float4 Color : COLOR0;
		};
	SSceneVertexToPixel ShadowedSceneVertexShader( float4 inPos : POSITION, float2 inTexCoords : TEXCOORD0, float3 inNormal : NORMAL)
		{
			SSceneVertexToPixel Output = (SSceneVertexToPixel)0;
			float4x4 preWorldViewProjection = mul (xWorld, xViewProjection);
			Output.Position = mul(inPos, preWorldViewProjection);
			Output.Pos2DAsSeenByLight= mul(inPos, xLightsWorldViewProjection);
			Output.Normal = normalize(mul(inNormal, (float3x3)xWorld));
			Output.Position3D = mul(inPos, xWorld);
			Output.TexCoords = inTexCoords;

			return Output;
		}
	SScenePixelToFrame ShadowedScenePixelShader(SSceneVertexToPixel PSIn)
		{
			SScenePixelToFrame Output = (SScenePixelToFrame)0;

			float2 ProjectedTexCoords;
			ProjectedTexCoords[0] = PSIn.Pos2DAsSeenByLight.x/PSIn.Pos2DAsSeenByLight.w/2.0f +0.5f;
			ProjectedTexCoords[1] = -PSIn.Pos2DAsSeenByLight.y/PSIn.Pos2DAsSeenByLight.w/2.0f +0.5f;

			float diffuseLightingFactor = 0;
			if ((saturate(ProjectedTexCoords).x == ProjectedTexCoords.x) && (saturate(ProjectedTexCoords).y == ProjectedTexCoords.y))
				{
					float depthStoredInShadowMap = tex2D(ShadowMapSampler, ProjectedTexCoords).r;
					float realDistance = PSIn.Pos2DAsSeenByLight.z/PSIn.Pos2DAsSeenByLight.w;
					if ((realDistance - 0.01f) <= depthStoredInShadowMap)
						{
							diffuseLightingFactor = DotProduct(xLightPos, PSIn.Position3D, PSIn.Normal);
							diffuseLightingFactor = saturate(diffuseLightingFactor);
							diffuseLightingFactor *= xLightPower[0];

							float lightTextureFactor = tex2D(CarLightSampler, ProjectedTexCoords).r;
							diffuseLightingFactor *= lightTextureFactor;
						}
					}
			PSIn.TexCoords.y--;
			float4 baseColor = tex2D(TextureSampler, PSIn.TexCoords);
			Output.Color = baseColor*(diffuseLightingFactor + xAmbient);
			
			
	//Lamp point light code - we do this for both lamps
	float3 lightDir0 = normalize(xLamppostPos[0] - PSIn.Position3D);
	float3 lightDir1 = normalize(xLamppostPos[1] - PSIn.Position3D);
	float diffuse0 = saturate(dot(normalize(PSIn.Normal), lightDir0));
	float diffuse1 = saturate(dot(normalize(PSIn.Normal), lightDir1));

	float d0 = distance(xLamppostPos[0], PSIn.Position3D);
	float d1 = distance(xLamppostPos[1], PSIn.Position3D);

	float att0 = 1 - pow(clamp(d0 / xLightAttenuation, 0, 1), xLightFallOff);
	float att1 = 1 - pow(clamp(d1 / xLightAttenuation, 0, 1), xLightFallOff);

	Output.Color += (diffuse0 * att0) + (diffuse1 * att1);

	// Lamp emissive light code (this handles the "aura" of both real and shadow lights)
	float4 screenPos = mul(PSIn.Position3D, xViewProjection);
	screenPos /= screenPos.w;
	for (int CurrentLight=0; CurrentLight<4; CurrentLight++)
		{
			float dist = distance(screenPos.xy, xLtscrPositions[CurrentLight].xy);
			if (dist < xCamRads[CurrentLight])
				{
					Output.Color.rgb += (xCamRads[CurrentLight]-dist)*4.0f;
				}
		}

			return Output;
		}

	technique ShadowedScene
		{
			pass Pass0
				{
					VertexShader = compile vs_3_0 ShadowedSceneVertexShader();
					PixelShader = compile ps_3_0 ShadowedScenePixelShader();
				}
		}
