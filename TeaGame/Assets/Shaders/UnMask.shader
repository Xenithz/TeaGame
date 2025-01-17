﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/UnMask"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100

		Pass
		{
			Stencil{
				Ref 1
				Comp Equal
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragment

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
			
			sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _Color;

			v2f vert (appdata v)
            {
                v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
            }


            fixed4 fragment (v2f i) : SV_Target
            {
				fixed4 col = tex2D(_MainTex, i.uv);

				return col*_Color;
            }
            ENDCG
		}
	}
}