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
			// You can remove these two lines,
			// to save some instructions. They're just
			// here for visual fidelity.

			// Translucency.
			half3 transLightDir = lightDir;
			float transDot = pow ( max (0, dot ( viewDir, -transLightDir ) ), 1f );
			fixed3 transLight = (atten * 2) * ( transDot ) * s.Alpha;
			fixed3 transAlbedo = s.Albedo * _LightColor0.rgb * transLight;

			// Regular BlinnPhong. <- cambie por diffuse
			half3 h = normalize (lightDir + viewDir);
			fixed diff = max (0, dot (s.Normal, lightDir));
			float nh = max (0, dot (s.Normal, h));
			//float spec = pow (nh, s.Specular*128.0) * s.Gloss;
			//fixed3 diffAlbedo = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb) * (atten * 2);
		    fixed3 diffAlbedo = (s.Albedo * _LightColor0.rgb * diff) * (atten * 2);
			// Add the two together.
			fixed4 c;
			c.rgb = diffAlbedo + transAlbedo;
			c.a = _LightColor0.a * s.Alpha * atten;
			return c;
		}

		ENDCG
	}
	FallBack "Transparent/VertexLit"
}