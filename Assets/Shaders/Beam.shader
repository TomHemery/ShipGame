Shader "Sprites/Beam"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_AlphaTex("Sprite Alpha", 2D) = "defaulttexture" {}
		_PixelsPerUnit("Pixels Per Unit", Int) = 0
		_Length("Length", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
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
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma shader_feature ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			uniform float _Length;
			uniform int _PixelsPerUnit;
			fixed4 _Color;

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
			};


			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(float4(IN.vertex.x, IN.vertex.y  * _Length, IN.vertex.z, IN.vertex.w));
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;

				return OUT;
			}

			fixed4 SampleSpriteTexture(float2 uv)
			{
				float yPos = _Length * float(_PixelsPerUnit) * uv.y;
				int texYPos = fmod(int(yPos), _PixelsPerUnit);
				yPos = float(texYPos) / float(_PixelsPerUnit);
				float2 mainTexUV = float2(uv.x, yPos);
				fixed4 color = tex2D(_MainTex, mainTexUV);
				color.a = tex2D(_AlphaTex, uv).a * color.a;
				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{	
				fixed4 c = SampleSpriteTexture(IN.texcoord);
				c.rgb *= c.a;
				return c;
			}

			ENDCG
		}
	}
}