Shader "Custom/GreyscaleShader" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_EffectAmount ("Effect Amount", Range(0,1)) = 1
	}
	SubShader {

	 	 Pass {
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			 
			#include "UnityCG.cginc"
			 
			sampler2D _MainTex;
			uniform float _EffectAmount;
			struct v2f 
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			 
			float4 _MainTex_ST;
			 
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				return o;
			}
			 
			half4 frag (v2f i) : COLOR
			{
				half4 texcol = tex2D (_MainTex, i.uv);
				texcol.rgb = lerp(texcol.rgb, dot(texcol.rgb, float3(0.3, 0.59, 0.11)), _EffectAmount);
				return texcol;
			}
			ENDCG
		 
		}
	} 
	FallBack "VertexLit"
}
