Shader "Unlit/LutSSS"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Lut("lut",2D)="white"{}
        _Color("Color",Color)=(1,1,1,1)
        _SSS("sss",Range(0,10))=1
        _GG("gg",float)=1
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
                float3 normal:NORMAL0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal:TEXCOORD1;
                float3 worldPos:TEXCOORD2;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Lut;
            float4 _Color;
            float _SSS;
            float _GG;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal=UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                float3 N=i.normal;
                float3 L=normalize(_WorldSpaceLightPos0.xyz-i.worldPos);
                float3 V=normalize(_WorldSpaceCameraPos.xyz-i.worldPos);
                float ddn=length(fwidth(i.normal));
                float ddwp=length(fwidth(i.worldPos));
                float curvature=ddn/ddwp;
                float halfLam=dot(N,L)*0.499+0.5;
                // sample the texture
                fixed4 lut = tex2D(_Lut, float2(halfLam,_SSS));
                fixed4 col =_Color;// tex2D(_MainTex, i.uv);
                float4 gg=max(0,pow( dot(V,normalize( N+L)),_GG));

                //col*=halfLam;//*lut;
                col*=lut*_Color;
                col+=gg;
                // apply fog
                return col;
            }
            ENDCG
        }
    }
}
