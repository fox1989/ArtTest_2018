Shader "Unlit/KeepOut"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Noise("Noise",2D)="white"{}
        _F("F",float) =1
        _Dis("dis",float)=1
        _DisF("disf",float)=1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100

        Pass
        {

            Blend SrcAlpha OneMinusSrcAlpha

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
                float4 scenePos:TEXCOORD1;
                float4 worldPos:TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Noise;
            float _F;
            float _Dis;
            float _DisF;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.scenePos=ComputeGrabScreenPos(o.vertex);
                o.worldPos=mul(unity_ObjectToWorld,o.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {


                float3 screenPos=  i.scenePos.xyz/i.scenePos.w;

                float2 dir=float2(0.5,0.5)-screenPos.xy;
                float dis=sqrt(dir.x*dir.x+dir.y*dir.y);
                dis+=0.5;
                //dis*=_F;
                float4 noise= tex2D(_Noise,i.uv);
                float vdis=length(i.worldPos-_WorldSpaceCameraPos.xyz);
                vdis=max(0,(vdis-_Dis)/_Dis)*_DisF;
                //return float4(dis,dis,dis,1);
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                dis=dis*vdis*_F;
                //col.a=dis;
                if(dis<noise.r)
                    col.a=lerp(0,noise.r,dis);

                return col;
            }
            ENDCG
        }
    }
}
