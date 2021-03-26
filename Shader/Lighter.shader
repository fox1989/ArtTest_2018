Shader "Unlit/Lighter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Scal("Scal",float) = 1
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
            float _Scal;

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
                fixed4 col = tex2D(_MainTex, i.uv);

                float2 uv=i.uv;
                float2 center=float2(0.5,0.5);
               
                float _RotateSpeed=10;
                uv = uv - center;  
                //旋转矩阵公式  
                uv = float2(uv.x * cos(_RotateSpeed * _Time.x) - uv.y * sin(_RotateSpeed * _Time.x),  
                            uv.x * sin(_RotateSpeed * _Time.x) + uv.y * cos(_RotateSpeed * _Time.x));  


                float2 dis=center-uv;

                float x=center.x+center.x*(-dis.x/center.x)*_Scal;
                float y=center.y+center.y*(-dis.y/center.y)*_Scal;

                float2 newUV=float2(x,y);
                newUV+=center;
                //newUV.x+=_Time.y;

                col=tex2D(_MainTex,newUV);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
