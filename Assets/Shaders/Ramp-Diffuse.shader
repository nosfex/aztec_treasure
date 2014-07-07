Shader "Custom/Ramp-Diffuse" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Ramp ("Shading Ramp (RGB)", 2D) = "white" {}
		_Shininess ("Shininess", Range (0.03, 1)) = 0.03
	}
	SubShader {
		Tags {"Queue"="Transparent-10" "RenderType"="Opaque"}
		//Cull Off
		LOD 200

		CGPROGRAM
		#pragma surface surf Ramp fullforwardshadows
	//	#pragma exclude_renderers flash

		sampler2D _MainTex;//, _BumpMap, _Thickness;
		sampler2D _Ramp;
		fixed4 _Color;
		//fixed4 _AddColor;
		half _Shininess;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.rgb * _Color.rgb;
			//o.Specular = tex.a * _Shininess;
			o.Gloss = tex.a * _Shininess;
			//o.Alpha = tex.a;//tex2D(_Thickness, IN.uv_MainTex).r;
			//o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
		}

		inline fixed4 LightingRamp (SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
		{		
		
	        half3 h = normalize (lightDir + viewDir);
	        float nh = max (0, dot (s.Normal, h));
    	    float spec = pow (nh, 48.0);

			half NdotL = dot (s.Normal, lightDir);
        	half diff = NdotL * 0.5 + 0.5;
        	half3 ramp = tex2D (_Ramp, float2(diff)).rgb;
        	half4 c;
        	//+ _LightColor0.rgb * spec
        	c.rgb = (s.Albedo * _LightColor0.rgb * ramp + ((_LightColor0.rgb * spec) * s.Gloss) ) * (atten * 2);
        	c.a = s.Alpha;
        	return c;
		}

		ENDCG
	}
	FallBack "Transparent/VertexLit"
}