Shader "Sprites/CircleMaskedSprite"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
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

			float4 frag(v2f IN) : SV_Target
			{
				float edge_dist = 1.0 - length(IN.texcoord - float2(0.5, 0.5)) * 2;
				float pixel_width = length(float2(ddx(edge_dist), ddy(edge_dist)));
				float mask = saturate(edge_dist / pixel_width);

				float4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
				c.a *= mask;
				c.rgb *= c.a;
				return c;
			}

		ENDCG
		}
	}
}
