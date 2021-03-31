Shader "fox/LowPolyWater"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color",Color) =(1,1,1,1)
		_DepthColor("DepthColor",Color)=(1,1,1,1)
		
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }//"Queue"="Opaque" "Transparent" }
        LOD 100
		GrabPass{"_GrabTex"}

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
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float3 worldPos:TEXCOORD1;
				float3 normal : TEXCOORD2;
				float4 screenPos:TEXCOORD3;
				float4 scrPos:TEXCOORD4;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _Color;
			sampler2D _CameraDepthTexture;
			float4 _DepthColor;
			sampler2D _GrabTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal=UnityObjectToWorldNormal(v.normal);
				o.worldPos=mul(unity_ObjectToWorld,v.vertex);
				o.screenPos=ComputeScreenPos(o.vertex);
				o.scrPos=ComputeGrabScreenPos(o.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
          //tex2D(_MainTex, i.uv);

				float4 screenPosNDC=i.screenPos/i.screenPos.w;

				float depth=tex2Dproj(_CameraDepthTexture,i.screenPos).r;
				float depth2=tex2D(_CameraDepthTexture,screenPosNDC.xy).r;
				float depth3=SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,screenPosNDC.xy);


				float linearEyeDepth=LinearEyeDepth(depth2); //不透明深度
				float linear01Depth = Linear01Depth(depth3);
				float t_depth=i.screenPos.w;
				float ff=linearEyeDepth- t_depth;
				
				float depthLerp=saturate(ff/0.2);
				ff=1-depthLerp;

     			fixed4 col =lerp(_Color,_DepthColor,saturate(linearEyeDepth*0.05));
			
				//return float4(ff,ff,ff,col.a);
				col.a=linearEyeDepth*0.1;
				float3 L=normalize( _WorldSpaceLightPos0.xyz);
				float3 V=normalize( _WorldSpaceCameraPos.xyz-i.worldPos);
				float3 N=i.normal;
				float df=dot(L,N);
				col.rgb*=df;
				float sp=dot(normalize( L+N),V);
				//col.rgb+=float3(1,1,1)*sp;
				col.rgb+=float3(1,1,1)*ff;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
				float4 f4=float4(i.normal,1);
				float4 scrCol=tex2Dproj(_GrabTex,i.scrPos+f4*0.1)*1.5;

                return col*scrCol;
            }
            ENDCG
        }
    }
}
