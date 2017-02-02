

Shader "Noise/ImprovedPerlinNoise3DReversedNormals" 
{

 Properties {
     _Color ("Main Color", Color) = (1,1,1,1)
     _MainTex ("Base (RGB)", 2D) = "white" {}
     _BumpMap ("Normalmap", 2D) = "bump" {}
 }
 
	SubShader 
	{
	    Tags { "RenderType"="Opaque" }
     	LOD 300
     	Cull Off
     	
	    Pass 
	    {
	
			CGPROGRAM
			#pragma surface surf Lambert
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			
			uniform sampler2D _PermTable2D, _Gradient3D;
			uniform float _Frequency, _Lacunarity, _Gain;
			
			sampler2D _MainTex;
 			sampler2D _BumpMap;
 			fixed4 _Color;
 			
 			 struct Input 
 			 {
			     float2 uv_MainTex;
			     float2 uv_BumpMap;
			 };
			 
			 void surf (Input IN, inout SurfaceOutput o) {
			     fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			     o.Albedo = c.rgb;
			     o.Alpha = c.a;
			     o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			 }
			
			struct v2f 
			{
			    float4 pos : SV_POSITION;
			    float4 uv : TEXCOORD;
			};
			

			
			v2f vert (appdata_base v)
			{
			    v2f o;
			    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			    o.uv = v.vertex;
			    return o;
			}
			
			float3 fade(float3 t)
			{
				return t * t * t * (t * (t * 6 - 15) + 10); // new curve
				//return t * t * (3 - 2 * t); // old curve
			}
			
			float4 perm2d(float2 uv)
			{
				return tex2D(_PermTable2D, uv);
			}
			
			float gradperm(float x, float3 p)
			{
				float3 g = tex2D(_Gradient3D, float2(x, 0) ).rgb *2.0 - 1.0;
				return dot(g, p);
			}
						
			float inoise(float3 p)
			{
				float3 P = fmod(floor(p), 256.0);	// FIND UNIT CUBE THAT CONTAINS POINT
			  	p -= floor(p);                      // FIND RELATIVE X,Y,Z OF POINT IN CUBE.
				float3 f = fade(p);                 // COMPUTE FADE CURVES FOR EACH OF X,Y,Z.
			
				P = P / 256.0;
				const float one = 1.0 / 256.0;
				
			    // HASH COORDINATES OF THE 8 CUBE CORNERS
				float4 AA = perm2d(P.xy) + P.z;
			 
				// AND ADD BLENDED RESULTS FROM 8 CORNERS OF CUBE
			  	return lerp( lerp( lerp( gradperm(AA.x, p ),  
			                             gradperm(AA.z, p + float3(-1, 0, 0) ), f.x),
			                       lerp( gradperm(AA.y, p + float3(0, -1, 0) ),
			                             gradperm(AA.w, p + float3(-1, -1, 0) ), f.x), f.y),
			                             
			                 lerp( lerp( gradperm(AA.x+one, p + float3(0, 0, -1) ),
			                             gradperm(AA.z+one, p + float3(-1, 0, -1) ), f.x),
			                       lerp( gradperm(AA.y+one, p + float3(0, -1, -1) ),
			                             gradperm(AA.w+one, p + float3(-1, -1, -1) ), f.x), f.y), f.z);
			}
			
			// fractal sum, range -1.0 - 1.0
			float fBm(float3 p, int octaves)
			{
				float freq = _Frequency, amp = 0.5;
				float sum = 0;	
				for(int i = 0; i < octaves; i++) 
				{
					sum += inoise(p * freq) * amp;
					freq *= _Lacunarity;
					amp *= _Gain;
				}
				return sum;
			}
			
			// fractal abs sum, range 0.0 - 1.0
			float turbulence(float3 p, int octaves)
			{
				float sum = 0;
				float freq = _Frequency, amp = 1.0;
				for(int i = 0; i < octaves; i++) 
				{
					sum += abs(inoise(p*freq))*amp;
					freq *= _Lacunarity;
					amp *= _Gain;
				}
				return sum;
			}
			
			// Ridged multifractal, range 0.0 - 1.0
			// See "Texturing & Modeling, A Procedural Approach", Chapter 12
			float ridge(float h, float offset)
			{
			    h = abs(h);
			    h = offset - h;
			    h = h * h;
			    return h;
			}
			
			float ridgedmf(float3 p, int octaves, float offset)
			{
				float sum = 0;
				float freq = _Frequency, amp = 0.5;
				float prev = 1.0;
				for(int i = 0; i < octaves; i++) 
				{
					float n = ridge(inoise(p*freq), offset);
					sum += n*amp*prev;
					prev = n;
					freq *= _Lacunarity;
					amp *= _Gain;
				}
				return sum;
			}
			
			half4 frag (v2f i) : COLOR
			{
				//uncomment this for fractal noise
				float n = fBm(i.uv.xyz, 4);

				//uncomment this for turbulent noise
				//float n = turbulence(i.uv.xyz, 4);
				
				//uncomment this for ridged multi fractal
				//float n = ridgedmf(i.uv.xyz, 4, 1.0);
			
			    return half4(n,n,n,1);
			}
			
			ENDCG
	
	    }
	}
	Fallback "VertexLit"
}

