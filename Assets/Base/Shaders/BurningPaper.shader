// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/BurningPaper"
{
	Properties{
		_MainTex("Main texture", 2D) = "white" {}
		_DissolveTex("Dissolution texture", 2D) = "gray" {}
		_Threshold("Threshold", Range(0, 1.1)) = 0
	}

		SubShader{

			Tags {"LightMode" = "ForwardBase"}

			Pass {


			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 diff : COLOR0;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata_base v) {
				v2f o;
				o.uv = v.texcoord;
				o.pos = UnityObjectToClipPos(v.vertex);     
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.diff = nl * _LightColor0;

				o.diff.rgb += ShadeSH9(half4(worldNormal, 1));
				return o;
			}

			sampler2D _DissolveTex;
			float _Threshold;

			fixed4 frag(v2f i) : SV_Target {
				fixed4 col = tex2D(_MainTex, i.uv);
				col *= i.diff;

				fixed4 c = tex2D(_MainTex, i.uv);
				fixed val = 1 - tex2D(_DissolveTex, i.uv).r;
				if (val < _Threshold - 0.04)
				{
					return i.diff;
				}
					bool b = val < _Threshold;
					col = lerp(col, col * fixed4(0, 0, lerp(1, 0, 1 - saturate(abs(_Threshold - val) / 0.04)), 1), b);
					
					return col;
				}

					ENDCG

			}

		}
			FallBack "Diffuse"
}