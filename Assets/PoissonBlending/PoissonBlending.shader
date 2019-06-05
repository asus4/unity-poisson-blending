/*
Poisson Blending
http://cs.brown.edu/courses/csci1950-g/results/proj2/edwallac/
*/
Shader "PoissonBlending"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_BlendTex("Texture", 2D) = "white" {}
		_MaskTex("Texture", 2D) = "black" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	uniform sampler2D _MainTex;
	uniform float4 _MainTex_ST;
	uniform float4 _MainTex_TexelSize;

	uniform sampler2D _BlendTex;
	uniform float4 _BlendTex_ST;
	uniform float4 _BlendTex_TexelSize;

	uniform sampler2D _MaskTex;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		return o;
	}

	#define tex2D_Main(uv) tex2D(_MainTex, i.uv + uv * _MainTex_TexelSize.xy );
	#define tex2D_Blend(uv) tex2D(_BlendTex, i.uv + uv * _BlendTex_TexelSize.xy );


	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 base = tex2D(_MainTex, i.uv);
		fixed mask = tex2D(_MaskTex, i.uv).r;

		if (mask < 0.5) {
			return base;
		}


		float2 next[4];
		next[0] = float2(-1.0, 0.0);
		next[1] = float2(1.0, 0.0);
		next[2] = float2(0.0, -1.0);
		next[3] = float2(0.0, 1.0);

		fixed3 blend = tex2D(_BlendTex, i.uv).rgb;
		fixed3 c = float3(0.0, 0.0, 0.0);

		for (int n = 0; n < 4; n++)
		{
			float2 uv = next[n];
			c += blend - tex2D_Blend(uv);
			if (mask < 0.5)
			{
				c += tex2D_Main(uv);
			}
			else
			{
				c += c;
			}
		}

		c /= 4;
		//
		base.rgb = c;
		return base;
	}

	ENDCG

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}
