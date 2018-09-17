// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Stencil"
{
    Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "Queue"="Geometry-1" "RenderType"="Transparent"}
		ColorMask 0
		ZWrite Off
		
		Pass
		{
			Stencil
			{
				Ref 1
				Comp Always
				Pass Replace
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragment
			float4 vert (float4 vertex : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(vertex);
            }
            
            fixed4 _Color;


            fixed4 fragment () : SV_Target
            {
				_Color.a=0;
                return _Color; 
            }
            ENDCG
		}
	}
}