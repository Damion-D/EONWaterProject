// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/YellowEffectRing"
{
	Properties
	{
		_Color("Ring Color", Color) = (1,1,0,0.75)
		_Speed("Animation Speed", Float) = 1
		_LineWidth("Line Width", Float) = 1
	}
		SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
		}


		   Pass {
		Blend SrcAlpha OneMinusSrcAlpha
		Zwrite off
		CGPROGRAM

		#pragma vertex vert             
		#pragma fragment frag

		struct vertInput {
			float4 pos : POSITION;
		};

		struct vertOutput {
			float4 pos : SV_POSITION;
			float3 worldPos : TEXCOORD0;
		};

		vertOutput vert(vertInput input) {
			vertOutput o;
			o.pos = UnityObjectToClipPos(input.pos);
			o.worldPos = mul(unity_ObjectToWorld, input.pos);
			return o;
		}

		float4 _Color;
		float _Speed;
		float _LineWidth;

		float4 frag(vertOutput output) : COLOR {
			float4 color = _Color;
			color.a = sin((_Time.y * _Speed) + (output.worldPos.y / _LineWidth)) * _Color.a;
			return color;
		}
		ENDCG
	}
    }
}
