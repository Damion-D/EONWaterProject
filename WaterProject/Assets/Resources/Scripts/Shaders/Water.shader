Shader "Custom/UnlitWaterShader"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_EdgeColor("Edge Color", Color) = (1, 1, 1, 1)
		_DepthFactor("Depth Factor", float) = 1.0
		_WaveSpeed("Wave Speed", float) = 1.0
		_WaveAmp("Wave Amp", float) = 0.2
		_DepthRampTex("Depth Ramp", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_MainTex("Main Texture", 2D) = "white" {}
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
			}

			Pass
			{
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#include "UnityCG.cginc"

				#pragma vertex vert
				#pragma fragment frag

					// Properties
					float4 _Color;
					float4 _EdgeColor;
					float  _DepthFactor;
					float  _WaveSpeed;
					float  _WaveAmp;
					float _ExtraHeight;
					sampler2D _CameraDepthTexture;
					sampler2D _DepthRampTex;
					sampler2D _NoiseTex;
					sampler2D _MainTex;

					struct vertexInput
					{
						float4 vertex : POSITION;
						float4 texCoord : TEXCOORD1;
					};

					struct vertexOutput
					{
						float4 pos : SV_POSITION;
						float4 texCoord : TEXCOORD0;
						float4 screenPos : TEXCOORD1;
					};

					vertexOutput vert(vertexInput input)
					{
						vertexOutput output;

						// convert to world space
						output.pos = UnityObjectToClipPos(input.vertex);

						// apply wave animation
						float noiseSample = tex2Dlod(_NoiseTex, float4(input.texCoord.xy, 0, 0));
						output.pos.y += sin(_Time*_WaveSpeed*noiseSample)*_WaveAmp;
						output.pos.x += cos(_Time*_WaveSpeed*noiseSample)*_WaveAmp;

						// compute depth
						output.screenPos = ComputeScreenPos(output.pos);

						// texture coordinates 
						output.texCoord = input.texCoord;

						return output;
					}

					float4 frag(vertexOutput input) : COLOR
					{
						// apply depth texture
						float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, input.screenPos);
						float depth = LinearEyeDepth(depthSample).r;

						// create foamline
						float foamLine = 1 - saturate(_DepthFactor * (depth - input.screenPos.w));
						float4 foamRamp = float4(tex2D(_DepthRampTex, float2(foamLine, 0.5)).rgb, 1.0);

						// sample main texture
						float4 albedo = tex2D(_MainTex, input.texCoord.xy);

						float4 col = _Color * foamRamp * albedo;
						return col;
					}

						
					ENDCG
				}
		}
}
/*{
	SubShader{
		Tags
		{
			"Queue" = "Transparent"
		}


		pass {
		CGPROGRAM
		#include "UnityCG.cginc"

		#pragma vertex vert
		#pragma fragment frag

			sampler2D _CameraDepthTexture;

		struct VertexInput {
			float4 vertex : POSITION;
		};

		struct VertexOutput {
			float4 pos : SV_POSITION;
			float4 screenPos : TEXCOORD1;
		};

		VertexOutput vert(VertexInput input)
		{
			VertexOutput output;

			output.pos = UnityObjectToClipPos(input.vertex);

			output.screenPos = ComputeScreenPos(output.pos);

			return output;
		}

		float4 frag(VertexOutput input) : COLOR
		{
			float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, input.screenPos);
			float depth = LinearEyeDepth(depthSample).r;

			float4 foamLine = float4(depth, depth, depth, 0.5);

			return foamLine;
		}
			ENDCG
}
	}

}*/
