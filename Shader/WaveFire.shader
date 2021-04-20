Shader "Unlit/WaveFire"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _UVTex("UvText",2D) = "white"{}
        _Level("Level",float)=0.1
        _OffsetX("offsetX",float)=0
        _OffsetY("offsetY",float)=0

        _Speed("Speed",float)=1
        //_Offset("Offset",Float2)=(0.0,0.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _UVTex;
            float _Level;
            float _OffsetX;
            float _OffsetY;
            float _Speed;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
               float2 uv= i.uv+float2(_Time.x,_Time.x)*_Speed;
               fixed4 tuv= tex2D(_UVTex,uv);
                tuv*=_Level;
                uv=i.uv+tuv.xy;
                uv-=float2(_OffsetX,_OffsetY)*_Level;

                fixed4 col = tex2D(_MainTex,uv);
                fixed4 col1 = tex2D(_MainTex,uv);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
