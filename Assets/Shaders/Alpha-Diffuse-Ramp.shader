//    Properties {
//      _MainTex ("Texture", 2D) = "white" {}
//      _BumpMap ("Bumpmap", 2D) = "bump" {}
//    }
//    SubShader {
//      Tags { "RenderType" = "Opaque" }
//      CGPROGRAM
//      #pragma surface surf Lambert
//      struct Input {
//        float2 uv_MainTex;
//        float2 uv_BumpMap;
//      };
//      sampler2D _MainTex;
//      sampler2D _BumpMap;
//      void surf (Input IN, inout SurfaceOutput o) {
//        o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
//        o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
//      }
Shader "Custom/Transparent-Diffuse-CullOff-Ramp" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
        _BumpMap ("Bumpmap", 2D) = "bump" {}
		_AddColor ("Add Color", Color) = (0,0,0,0)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Ramp ("Shading Ramp (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		//Cull Off
		LOD 200

		CGPROGRAM
		#pragma surface surf Translucent alpha halfasview 
		#pragma exclude_renderers flash

		sampler2D _MainTex;
        sampler2D _BumpMap;
		sampler2D _Ramp;
        
		fixed4 _Color;
		fixed4 _AddColor;
		//half _Shininess;

		struct Input {
			float2 uv_MainTex;
	        float2 uv_BumpMap;
	        float4 screenPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			//IN.uv_MainTex.x /= 2.0;
			//IN.uv_MainTex.y /= 2.0;
			
			//IN.uv_MainTex.x = (int)IN.uv_MainTex.x;
			//IN.uv_MainTex.y = (int)IN.uv_MainTex.y;

			//IN.uv_MainTex.x *= 2.0;
			//IN.uv_MainTex.y *= 2.0;			
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.rgb * _Color.rgb;
			o.Alpha = tex.a;//tex2D(_Thickness, IN.uv_MainTex).r;
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
			

			//o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
		}

		inline fixed4 LightingTranslucent (SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
		{	

			
			lightDir.z = max( 0.5, lightDir.z +0.2);
			
			lightDir.y += .5;
			half NdotL = dot (s.Normal, lightDir);
        	half diff = NdotL;// * 0.5 + 0.5;//max (0, length(viewDir - lightDir));
        	half3 ramp = tex2D (_Ramp, float2(0,diff)).rgb;
        	//half4 c;
        //c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
		
			
			//fixed diff = max (0, length(viewDir - lightDir));
		    fixed3 diffAlbedo = (s.Albedo * _LightColor0.rgb * ramp) * (atten * 1);
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