// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Punch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PunchPos("PunchPos",Vector)=(1,1,1,1)
        _Specular("specular",float)=1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag
       

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal:NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal:TEXCOORD1;
                float3 worldPos:TEXCOORD2;
                SHADOW_COORDS(3)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _PunchPos;
            float _Specular;

            v2f vert (appdata v)
            {
                v2f o;

                float4 objPos=v.vertex;
                float dis=distance(objPos.xyz,_PunchPos.xyz);

                if(dis<_PunchPos.w)
                {
                    float3 dir=normalize(v.normal)*-1;
                    float deep=(_PunchPos.w-dis)/_PunchPos.w;
                    deep=log(deep+1);
                    objPos.xyz+= dir*deep;
                }
                o.pos = UnityObjectToClipPos(objPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal=UnityObjectToWorldDir(v.normal);
                o.worldPos=mul(unity_ObjectToWorld,v.vertex).xyz;


                TRANSFER_SHADOW(o);
                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                float3 N=normalize(i.worldNormal);
                float3 L=normalize(_WorldSpaceLightPos0.xyz);
                float3 V=normalize(_WorldSpaceCameraPos.xyz-i.worldPos);
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                float3 diffuse=dot(N,L)*0.5+0.5;
                float3 specular=pow(max(0,dot(N,normalize(L+V))),_Specular);

                float shadow = SHADOW_ATTENUATION(i);

                col.rgb*=(ambient+diffuse+specular)*_LightColor0.rgb;
                
                col.rgb*=shadow;
         
                return col;
            }
            ENDCG
        }
        
       Pass
       {
            Tags
            {
                "LightMode" = "ForwardAdd"
            }
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd

         	#include "Lighting.cginc"
			#include "AutoLight.cginc"

            float4 _PunchPos;
            float _Specular;
            
           	struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			
			struct v2f {
				float4 position : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
			};
			
        

			v2f vert(a2v v) {
			 	v2f o;

                float4 objPos=v.vertex;
                float dis=distance(objPos.xyz,_PunchPos.xyz);

                if(dis<_PunchPos.w)
                {
                    float3 dir=normalize(v.normal)*-1;
                    float deep=(_PunchPos.w-dis)/_PunchPos.w;
                    deep=log(deep+1);
                    objPos.xyz+= dir*deep;
                }
                o.position = UnityObjectToClipPos(objPos);
			 	
			 	o.worldNormal = UnityObjectToWorldNormal(v.normal);
			 	
			 	o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			 	
			 	return o;
			}
			
			fixed4 frag(v2f i) : SV_Target {
				fixed3 worldNormal = normalize(i.worldNormal);
				#ifdef USING_DIRECTIONAL_LIGHT
					fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				#else
					fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
				#endif

			 	fixed3 diffuse = _LightColor0.rgb *  max(0, dot(worldNormal, worldLightDir));

			 	fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
			 	fixed3 halfDir = normalize(worldLightDir + viewDir);
			 	fixed3 specular = _LightColor0.rgb * pow(max(0, dot(worldNormal, halfDir)), _Specular);

				#ifdef USING_DIRECTIONAL_LIGHT
					fixed atten = 1.0;
				#else
					float3 lightCoord = mul(unity_WorldToLight, float4(i.worldPos, 1)).xyz;
					fixed atten = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
				#endif
			 	
				return fixed4((diffuse + specular) * atten, 1.0);
			}

            ENDCG
        }
    }
    FallBack "Specular"
}
