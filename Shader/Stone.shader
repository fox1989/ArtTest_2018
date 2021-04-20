// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Stone"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Threshold("Threshold",float)=0
        _StoneTex("StoneTex",2D)="white" {}
        _Gradual("_Gradual",float)=0.2
        _Specular("specular",float)=4
        _Noise("Noise",2D)="white"{}
    }
    SubShader
    {
        Tags { "RenderType"="Queue" }
        LOD 100
        //Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 noraml:NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 objectPos:TEXCOORD1;
                float3 noraml:TEXCOORD2;
                float3 worldPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Threshold;
            float _Gradual;
            float   _Specular;

            sampler2D _StoneTex;
            sampler2D _Noise;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.objectPos=v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.noraml=UnityObjectToWorldNormal(v.noraml);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 stone = tex2D(_StoneTex, i.uv);
                fixed4 noise=tex2D(_Noise,i.uv);
                if( i.objectPos.x<_Threshold)
                {
                   stone=float4(1,1,1,1);
                }else if( i.objectPos.x-_Gradual>_Threshold)
                {
                 
                }else
                {
                    float f=(i.objectPos.x-_Threshold)/_Gradual;
                  
                    stone= lerp(float4(1,1,1,1),stone,f);
                }
                float3 V=normalize(_WorldSpaceCameraPos.xyz-i.worldPos);
                float3 L=normalize(_WorldSpaceLightPos0.xyz);
                float3 N=i.noraml;
                float3 diffuse =_LightColor0.rbg*(max(0,dot(N,L)));
                diffuse=diffuse*0.5+0.5;
                float3 specular=_LightColor0.rbg*dot(normalize(reflect(-L, N)),V);
                specular=pow(saturate(specular),_Specular);
                col=col*stone*float4( diffuse,1)+float4(specular,1);

                // apply fog
                return col;
            }
            ENDCG
        }
    }
}
