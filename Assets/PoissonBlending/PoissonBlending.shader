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

	// Get gradient whti adjacent pixels 
	inline fixed3 gradient(sampler2D tex, float2 uv, float2 texel)
	{
		fixed3 c = fixed3(0.5, 0.5, 0.5);

		fixed3 center = tex2D(tex, uv).rgb;
		fixed3 right = center - tex2D(tex, uv + float2(texel.x, 0)).rgb;
		fixed3 left = center - tex2D(tex, uv + float2(-texel.x, 0)).rgb;
		fixed3 up = center - tex2D(tex, uv + float2(0, texel.y)).rgb;
		fixed3 down = center - tex2D(tex, uv + float2(0, -texel.y)).rgb;

		c += (right + left + up + down) * 2;
		return c;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 target = tex2D(_MainTex, i.uv);
		fixed mask = tex2D(_MaskTex, i.uv).r;

		if (mask < 0.5) {
			return target;
		}

		fixed3 src = tex2D(_BlendTex, i.uv).rgb;

		fixed3 gradSrc = gradient(_BlendTex, i.uv, _BlendTex_TexelSize.xy);
		fixed3 gradDst = gradient(_MainTex, i.uv, _MainTex_TexelSize.xy);
		fixed3 gradMix = (gradSrc + gradDst) * 0.5;

		src.r += ((gradSrc.r - gradDst.r) > 0.0 ? gradSrc.r : gradDst.r) - 0.5;
		src.g += ((gradSrc.g - gradDst.g) > 0.0 ? gradSrc.g : gradDst.g) - 0.5;
		src.b += ((gradSrc.b - gradDst.b) > 0.0 ? gradSrc.b : gradDst.b) - 0.5;
		
		//src /= 4.0;

		target.rgb = src;
		return target;
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
