Shader "Unlit/InkPainting"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Thresh("Thresh",float) =1
        _Tooniness("Tooniness",float)=1
        _RampTex("RampTex",2D) ="white"
        _Noise("noise",2D)="white"
        _OutLine("outline",float)=1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha


        Pass///Outline
        {
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 noraml:NORMAL0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 noraml:TEXCOORD1;
                float3 worldPos:TEXCOORD2;
            };   
            
            sampler2D _Noise;
            float _OutLine;
            v2f vert (appdata v)
            {
                v2f o;
                 float4 burn = tex2Dlod(_Noise, v.vertex);

			
				float3 scaledir = mul((float3x3)UNITY_MATRIX_MV, normalize(v.noraml.xyz));
				scaledir += 0.5;
				scaledir.z = 0.01;
				scaledir = normalize(scaledir);

				// camera space
				float4 position_cs = mul(UNITY_MATRIX_MV, v.vertex);
				position_cs /= position_cs.w;

				float3 viewDir = normalize(position_cs.xyz);
				float3 offset_pos_cs = position_cs.xyz + viewDir * 1;

				// y = cos（fov/2）
				float linewidth = -position_cs.z / (unity_CameraProjection[1].y);
				linewidth = sqrt(linewidth);
				position_cs.xy = offset_pos_cs.xy + scaledir.xy * linewidth * burn.x * _OutLine ;
				position_cs.z = offset_pos_cs.z;
				o.vertex = mul(UNITY_MATRIX_P, position_cs);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return float4(0.1,0.1,0.1,1);  
            }
            ENDCG

        }



              Pass///Outline2
        {
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 noraml:NORMAL0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 noraml:TEXCOORD1;
                float3 worldPos:TEXCOORD2;
            };   
            
            sampler2D _Noise;
            float4 _Noise_ST;
            float _OutLine;
            v2f vert (appdata v)
            {
                v2f o;
                 float4 burn = tex2Dlod(_Noise, v.vertex);

			
				float3 scaledir = mul((float3x3)UNITY_MATRIX_MV, normalize(v.noraml.xyz));
				scaledir += 0.5;
				scaledir.z = 0.01;
				scaledir = normalize(scaledir);

				// camera space
				float4 position_cs = mul(UNITY_MATRIX_MV, v.vertex);
				position_cs /= position_cs.w;

				float3 viewDir = normalize(position_cs.xyz);
				float3 offset_pos_cs = position_cs.xyz + viewDir * 2;

				// y = cos（fov/2）
				float linewidth = -position_cs.z / (unity_CameraProjection[1].y);
				linewidth = sqrt(linewidth);
				position_cs.xy = offset_pos_cs.xy + scaledir.xy * linewidth * burn.x * _OutLine *2;
				position_cs.z = offset_pos_cs.z;
				o.vertex = mul(UNITY_MATRIX_P, position_cs);
                o.uv=TRANSFORM_TEX(v.uv,_Noise);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
           
                float4 col=tex2D(_Noise,i.uv);
                if(col.r>0.5)
                    discard;
                return float4(0.1,0.1,0.1,1); 
            }
            ENDCG

        }


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
                float3 noraml:NORMAL0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 noraml:TEXCOORD1;
                float3 worldPos:TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Thresh;
            float _Tooniness;
            sampler2D _RampTex;
            sampler2D _Noise;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.noraml=UnityObjectToWorldNormal(v.noraml);
                o.worldPos=mul(unity_ObjectToWorld,v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 N=i.noraml;
                float3 V=normalize(_WorldSpaceCameraPos.xyz-i.worldPos);
                float edge=dot(V,N);
                edge=(edge>_Thresh)?1:edge*edge;
                float r=(col.r+col.g+col.b)*0.33;
                col.rgb=float3(r,r,r);
                fixed4 noise = tex2D(_Noise, i.uv);
                float3 L=normalize(_WorldSpaceLightPos0.xyz);

                float m=saturate(dot(N,L)+noise.r*0.3);
                float4 col2=  tex2D(_RampTex,float2((m+1)*0.5,0.5));
                col2.a=col2.r*_Tooniness;
                col2.rgb=col2.rgb*col.rgb;
                
                return col2;
               
            }
            ENDCG
        }
    }
}
