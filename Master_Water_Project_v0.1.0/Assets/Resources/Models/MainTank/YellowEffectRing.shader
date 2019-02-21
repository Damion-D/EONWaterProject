// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/YellowEffectRing"
{
	Properties
	{

		_Color("Ring Color", Color) = (1,1,0,0.75);
	_Speed("Animation Speed", Float) = 1;
	}
		SubShader
	{
	   Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				struct VertInput {
					float4 pos : POSITION;
				};

				struct VertOutput {
					float4 post : SV_POSITION;
				};

				VertOutput vert(VertInput input)
				{
					VertOutput o;
					o.pos = UnityObjectToClipPos(input.pos);
					return o;
				}

				float4 frag(VertexOutput output) : COLOR
				{
					 return (_Color.r, _Color.g, _Color.b, sin(_Time * _Speed) * _Color.a);
				}
					ENDCG
			};
    }
}
