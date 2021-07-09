Shader "Unlit/GeometryShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _R("R",float) =0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };


            struct v2g
            {
                float2 uv:TEXCOORD0;
                float4 vertex:SV_POSITION;
                float3 worldPos:TEXCOORD1;
            };

            struct g2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float dist:TEXCOORD1;
                float3 center:TEXCOORD2;
                float3 worldPos:TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _R;

            v2g vert (appdata v)
            {
                v2g o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.worldPos=mul(unity_ObjectToWorld,v.vertex);
                return o;
            }

            [maxvertexcount(3)]
            void geom(triangle v2g IN[3],inout TriangleStream <g2f> sriStream)
            {
                    float3 center=(IN[0].worldPos+IN[1].worldPos+IN[2].worldPos)/3.0;
                    g2f OUT;
                    OUT.vertex=IN[0].vertex;
                    OUT.uv=IN[0].uv;                
                    OUT.dist=distance(IN[0].worldPos.xyz,center.xyz);
                    OUT.center=center;
                    OUT.worldPos=IN[0].worldPos;
                    sriStream.Append(OUT);                

                    OUT.vertex=IN[1].vertex;
                    OUT.uv=IN[1].uv;                
                    OUT.dist=distance(IN[1].worldPos.xyz,center.xyz);
                    OUT.worldPos=IN[1].worldPos;
                    OUT.center=center;
                    sriStream.Append(OUT);  

                    OUT.vertex=IN[2].vertex;
                    OUT.uv=IN[2].uv;                
                    OUT.dist=distance(IN[2].worldPos.xyz,center.xyz);
                    OUT.worldPos=IN[2].worldPos;
                    OUT.center=center;
                    sriStream.Append(OUT);  
            }

            fixed4 frag (g2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
            
                float dis=distance(i.worldPos,i.center);
                if(dis<_R)
                    col.rgb=float3(1,0,0);
                //col.rgb=lerp(col.rgb,float3(1,0,0),i.dist);


                return col;
            }
            ENDCG
        }
    }
}
