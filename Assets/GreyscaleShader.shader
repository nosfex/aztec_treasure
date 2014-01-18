Shader "Custom/GreyscaleShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_EffectAmount ("Effect Amount", Range(0,1)) = 1
	}
	SubShader {
		Tags { "Queue"="Transparent" "LightMode"="Always" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		uniform float _EffectAmount;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = lerp(c.rgb, dot(c.rgb, float3(0.3, 0.59, 0.11)), _EffectAmount ) ;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Transparent/VertexLit"
}
