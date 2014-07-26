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
Shader "Custom/Normal-Diffuse-Ramp-Dithering" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
        _BumpMap ("Bumpmap", 2D) = "bump" {}
		_AddColor ("Add Color", Color) = (0,0,0,0)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Ramp ("Shading Ramp (RGB)", 2D) = "white" {}
		
		_PaletteHeight ("Palette Height", float) = 128
		_PaletteTex ("Palette", 2D) = "black" {}
		_DitherSize ("Dither Size (Width/Height)", float) = 8
		_DitherTex ("Dither", 2D) = "black" {}
		
	}
	SubShader {
		Tags {"Queue"="Geometry" "RenderType"="Opaque"}
		//Lighting Off
	
		//Cull Off
		LOD 200

		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Translucent fullforwardshadows vertex:vert finalcolor:dither
		#include "CGIncludes/Dithering.cginc"
		#include "UnityCG.cginc"

		sampler2D _MainTex;
        sampler2D _BumpMap;
		sampler2D _Ramp;
        
		sampler2D _PaletteTex;
		sampler2D _DitherTex;

		fixed4 _Color;
		fixed4 _AddColor;
		
		
		//half _Shininess;

		struct Input {
			float2 uv_MainTex;
	        float4 ditherPos;
	        float2 uv_BumpMap;
		};
		
		//float _ColorCount;
		float _PaletteHeight;
		float _DitherSize;
		
		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.ditherPos = GetDitherPos( v.vertex, _DitherSize);
		}		
		
		void dither(Input i, SurfaceOutput o, inout fixed4 color) {
			//color.rgb = ( GetDitherColor(color.rgb, _DitherTex, _PaletteTex,
			//	_PaletteHeight, i.ditherPos, _ColorCount) );
			//fixed4 c;
			//color.rgb = GetDitherColor(color.rgb, _DitherTex, _PaletteTex,
		    //  _PaletteHeight, i.ditherPos, _ColorCount);
			//color.rgb *= ();	
			//color.a *= 0.3;
			//color.rgb = fixed3(1,1,1);
			//color.rgb *= 
			//color.a *= 0.3;
			//color.rgb = i;
			//fixed dither = GetDitherColor(color.rgb, _DitherTex, _PaletteTex,
			//	      _PaletteHeight, i.ditherPos, 3);
			
			color.rgb *= GetDitherColor(color.rgb * 1, _DitherTex, _PaletteTex,
				      _PaletteHeight, i.ditherPos, 2);
		}

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

			
			lightDir.z = max( 0.5, lightDir.z + 0.2);
			
			lightDir.y += .5;
			half NdotL = dot (s.Normal, lightDir);
        	half diff = NdotL;// * 0.5 + 0.5;//max (0, length(viewDir - lightDir));
        	half3 ramp = tex2D (_Ramp, float2(0,diff)).rgb;
        	//half4 c;
        //c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
		
			
			//fixed diff = max (0, length(viewDir - lightDir));
		    fixed3 diffAlbedo = (s.Albedo * _LightColor0.rgb * ramp) * (atten * 2);
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