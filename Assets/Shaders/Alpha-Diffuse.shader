//Shader "Custom/Transparent-Diffuse-CullOff" {
//Properties {
//	_Color ("Main Color", Color) = (1,1,1,1)
//	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
//}
//
//SubShader {
//	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
//	LOD 200
//	Cull Off
//
//CGPROGRAM
//#pragma surface surf Lambert alpha
//
//sampler2D _MainTex;
//fixed4 _Color;
//
//struct Input {
//	float2 uv_MainTex;
//};
//
//void surf (Input IN, inout SurfaceOutput o) {
//	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
//	o.Albedo = c.rgb;
//	o.Alpha = c.a;
//}
//ENDCG
//}
//
//Fallback "Transparent/VertexLit"
//}
Shader "Custom/Transparent-Diffuse-CullOff" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_AddColor ("Add Color", Color) = (0,0,0,0)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		//Cull Off
		LOD 200

		CGPROGRAM
		#pragma surface surf Translucent alpha
		#pragma exclude_renderers flash

		sampler2D _MainTex;//, _BumpMap, _Thickness;
		fixed4 _Color;
		fixed4 _AddColor;
		//half _Shininess;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.rgb * _Color.rgb;
			o.Alpha = tex.a;//tex2D(_Thickness, IN.uv_MainTex).r;
			//o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
		}

		inline fixed4 LightingTranslucent (SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
		{		
			fixed diff = max (0, length(viewDir - lightDir));
		    fixed3 diffAlbedo = (s.Albedo * _LightColor0.rgb * diff) * (atten * 1);
			// Add the two together.
			fixed4 c;
			c.rgb = diffAlbedo + (_AddColor * 0.5);
			c.a = _LightColor0.a * s.Alpha * atten;
			return c;
		}

		ENDCG
	}
	FallBack "Transparent/VertexLit"
}