Shader "Unlit/Grow"
{
  Properties
  {
    _AnimateValues("Animate Values", Vector) = (0, 0, 0, 0)
    _Color("Color", Vector) = (1, 1, 1, 1)
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

        #define LINE_WIDTH 0.008
        #define AXIS_LENGTH 0.05
        #define AXIS_OFFSET 0.2
        sampler2D _MainTexture;
        float4 _AnimateValues;
        float4 _Color;

        v2f vert (appdata v)
        {
          v2f o;
          o.vertex = UnityObjectToClipPos(v.vertex);
          o.uv = v.uv;
          return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
          float centerDist = distance(i.uv, fixed2(0.5, 0.5));
          // circle
          fixed4 color = _Color;
          color.w = step(abs(centerDist - _AnimateValues.x), LINE_WIDTH);

          // axis
          fixed2 axisDist = fixed2(abs(i.uv.x - 0.5), abs(i.uv.y - 0.5));
          float isAxis = step(abs(axisDist.x - LINE_WIDTH), LINE_WIDTH) 
          + step(abs(axisDist.y - LINE_WIDTH), LINE_WIDTH);
          isAxis *= step(
            abs((_AnimateValues.x + AXIS_OFFSET) - centerDist), 
            AXIS_LENGTH);

          color.w += isAxis;

          color.w *= _AnimateValues.w;
          return color;
        }
        ENDCG
      }
  }
}
