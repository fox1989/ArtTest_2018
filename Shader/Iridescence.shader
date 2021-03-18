Shader "Unlit/Iridescence"
{
   Properties {
        _RainBow ("RainBow", 2D) = "white" {}
        _MainTex("MainTex",2D)="white"{}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _RainBow; uniform float4 _RainBow_ST;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float3 uv:TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float uv:TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                o.uv=TRANSFORM_TEX(v.uv,_MainTex);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float  NDotV = dot(normalDirection,viewDirection);
                float2 NDotV2 = float2(NDotV,NDotV);
                float4 tex = tex2D(_RainBow,TRANSFORM_TEX(NDotV2, _RainBow));
                float3 emissive = tex.rgb;
                float3 finalColor = emissive;
                float4 col= tex2D(_MainTex,i.uv);
                finalColor=finalColor*0.1;
            
                return float4(finalColor,1)+col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
