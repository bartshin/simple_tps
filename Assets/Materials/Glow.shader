Shader "Unlit/Grow"
{
  Properties
  {
    _MainTexture("Main Texture", 2D) = "White" {}
    _AnimateValues("Animate Values", Vector) = (0, 0, 0, 0)
  }
  SubShader
  {
    Tags {
      "Queue"="Transparent"
    }
    Blend SrcAlpha OneMinusSrcAlpha

    LOD 100

      Pass
      {
        CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members alpha)
#pragma exclude_renderers d3d11
        #pragma vertex vert
        #pragma fragment frag
        
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

        sampler2D _MainTexture;
        float4 _AnimateValues;

        v2f vert (appdata v)
        {
          v2f o;
          o.vertex = UnityObjectToClipPos(v.vertex);
          o.uv = v.uv;
          o.uv = (o.uv - fixed2(0.5, 0.5)) * _AnimateValues.x + fixed2(0.5, 0.5);
          o.uv -= fixed2(_AnimateValues.y, _AnimateValues.z);
          return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
          fixed4 textureColor = tex2D(_MainTexture, i.uv);
          textureColor.w *= _AnimateValues.w;
          return textureColor;
        }
        ENDCG
      }
  }
}
