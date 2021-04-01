Shader "Unlit/Projector"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color",Color) = (1,1,1,1)
        _DecalTex("Cookie",2D) = "" {}
        _FalloffTex("FallOff",2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {

            ZWrite Off
            ColorMask RGB
            Offset -1, -1

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
                float4 pos : SV_POSITION;
                float4 uvDecal:TEXCOORD1;
                float4 uvFalloff:TEXCOORD2;
        
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4x4 unity_Projector;
            float4x4 unity_ProjectorClip;
            sampler2D _DecalTex;
            sampler2D _FalloffTex;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);

                o.uvDecal=mul(unity_Projector,v.vertex);
                o.uvFalloff=mul(unity_ProjectorClip,v.vertex);



                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                	fixed4 decal = tex2Dproj (_DecalTex, UNITY_PROJ_COORD(i.uvDecal));
				decal *= _Color;
 
				fixed falloff = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff)).r;
				decal *= falloff;
				return decal;




                return col;
            }
            ENDCG
        }
    }
}
