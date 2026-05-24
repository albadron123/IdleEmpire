Shader "Custom/Alpha"
{
    Properties
    {
        _MainTex("Sprite A Texture", 2D) = "white" {}
        _AlphaSourceTex("Alpha Source Render Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1) // Default is White
        _Config ("COnfig", Float) = 1
    }

        SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
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
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _AlphaSourceTex;
            half4 _Color;
            half _Config;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 mainColor = tex2D(_MainTex, i.uv);

            // Sample the render texture at screen position
            float2 screenUV = i.screenPos.xy / i.screenPos.w;
            fixed4 alphaSource = tex2D(_AlphaSourceTex, screenUV);

            // Use alpha source's alpha (or grayscale) as Sprite A's alpha
            mainColor.a = _Color.a*(1-alphaSource.a) * _Config + (_Color.a * alphaSource.a * alphaSource.a) * (1-_Config);
            mainColor.rgb = _Color.rgb;
            // Or use the brightness of the alpha source
            // float brightness = dot(alphaSource.rgb, float3(0.299, 0.587, 0.114));
            // mainColor.a = brightness;

            return mainColor;
        }
        ENDCG
    }
    }
}
