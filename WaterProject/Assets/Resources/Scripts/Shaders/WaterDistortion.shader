Shader "Unlit/WaterDistortion"
{
	Properties
	{
		_WaveSpeed("Wave Speed", float) = 1.0
		_WaveAmp("Wave Amp", float) = 0.2
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_DistortStrength("Distort Strength", float) = 1.0
	}

	SubShader{
		// Grab the screen behind the object into _BackgroundTexture
			GrabPass
			{
				"_BackgroundTexture"
			}

		// Background distortion
		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			// Properties
			sampler2D _BackgroundTexture;
			sampler2D _NoiseTex;
			float     _DistortStrength;
			float  _WaveSpeed;
			float  _WaveAmp;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float3 texCoord : TEXCOORD0;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float4 grabPos : TEXCOORD0;
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				// convert input to world space
				output.pos = UnityObjectToClipPos(input.vertex);
				float4 normal4 = float4(input.normal, 0.0);
				float3 normal = normalize(mul(normal4, unity_WorldToObject).xyz);

				// use ComputeGrabScreenPos function from UnityCG.cginc
				// to get the correct texture coordinate
				output.grabPos = ComputeGrabScreenPos(output.pos);

				// distort based on bump map
				float noiseSample = tex2Dlod(_NoiseTex, float4(input.texCoord.xy, 0, 0));
				output.grabPos.y += sin(_Time*_WaveSpeed*noiseSample) * _WaveAmp * _DistortStrength;
				output.grabPos.x += cos(_Time*_WaveSpeed*noiseSample) * _WaveAmp * _DistortStrength;
				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				return tex2Dproj(_BackgroundTexture, input.grabPos);
			}
			ENDCG
		}
	}
}