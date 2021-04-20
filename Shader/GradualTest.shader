Shader "Unlit/GradualTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Ty("Ty",float)=1
        _f("F",float)=1
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
                float3 pos:TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Ty;
            float _f;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.pos=v.vertex;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);


                float f=saturate((_Ty-i.pos.y)*_f);
                col=lerp(col,col*float4(1,0,0,1),f);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    	Fallback "Diffuse"
}
