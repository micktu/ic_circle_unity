Shader "Sprites/CircleMaskedSprite"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_Thickness ("Inner width", Float) = 0
		_CircleColor ("Circle Color", Color) = (0.5,0.5,0.5,1)
		_Radius ("Radius", float) = 0
		_X ("X", float) = 0
		_Y ("Y", float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};
			
			float4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;

				return OUT;
			}

			sampler2D _MainTex;
			float _Thickness;
			float4 _CircleColor;
			float _Radius;
			float _X;
			float _Y;

			float4 frag(v2f IN) : SV_Target
			{
				float edge_dist, pixel_width, mask;
				float4 c = tex2D(_MainTex, IN.texcoord) * IN.color; 

				// pass 1: blend offset circle with background
				if (_Radius != 0)
				{
					edge_dist = 1 - length((IN.texcoord - float2(_X, _Y) - 0.5 + _Radius) / _Radius - 1);
					pixel_width = length(float2(ddx(edge_dist), ddy(edge_dist)));
					mask = saturate(edge_dist / pixel_width);
					c = lerp(c, _CircleColor, mask);
				}

				// pass 2: apply circle mask
				edge_dist = 1.0 - length(IN.texcoord - 0.5) * 2;
				pixel_width = length(float2(ddx(edge_dist), ddy(edge_dist)));
				mask = saturate(edge_dist / pixel_width);

				// pass 3: apply cutout
				if (_Thickness != 0)
				{
					mask -= saturate((edge_dist - pixel_width * (_Thickness + 1)) / pixel_width);
				}

				c.a *= mask;
				c.rgb *= c.a;
				
				return c;
			}

		ENDCG
		}
	}
}
