Shader "Unlit/SSR"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SkyBox("SkyBox",Cube)=""{}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        GrabPass {}
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
                float4 positionCS:TEXCOORD1;
                float3 positionWS:TEXCOORD2;
                float4 positionOS:TEXCOORD3;
                float4 vsRay:TEXCOORD4;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _CameraDepthTexture;
            samplerCUBE _SkyBox; 
            sampler2D _GrabTexture; //屏幕图片


            float2 ViewPosToCS(float3 pos)
            {
                float4 objPos=UnityWorldToClipPos(float4(pos,1));//mul(UNITY_MATRIX_I_V,float4(pos,1));
                //objPos = UnityObjectToClipPos(objPos);
                float4 screenPos = ComputeScreenPos(objPos);
                screenPos.xyz /= screenPos.w;
                //screenPos.xy = screenPos.xy * 0.5 + 0.5;
                return screenPos.xy;
            }


            float compareWithDepth(float3 worldPos)
            {
              /*  float2 uv = ViewPosToCS(vpos);

                float depth=SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,uv);
				depth = LinearEyeDepth(depth);

                int isInside = uv.x > 0 && uv.x < 1 && uv.y > 0 && uv.y < 1;
                return lerp(0, abs(vpos.z + depth), isInside);

        */


                fixed3 p = worldPos; //当前反射到的点的世界空间的坐标
                float pD =length(p - _WorldSpaceCameraPos.xyz); //当前点离摄像机的距离

                //当前点在屏幕上的投影坐标
                //fixed4 scrnCoord = ComputeScreenPos(UnityWorldToClipPos(fixed4(p, 1)));
                //除以w转化为uv坐标
                float2 screenUV = ViewPosToCS(worldPos);
                if(screenUV.x<0 || screenUV.y<0 || screenUV.x>1 || screenUV.y>1)
                {
                    //如果在画面外，返回否
                    screenUV = fixed2(-1, -1);
                    return 0;
                }
                //当前点所对应的屏幕空间点的深度值
                float cD = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV));
                
                return abs(pD) - abs(cD); //当点的深度大于摄像机深度，判定相交

            }

            bool rayMarching(float3 o, float3 dir, out float2 hitUV)
            {
                float3 end = o;
                float stepSize = 0.5;
                float thinkness = 0.1;
                float triveled = 0;
                int max_marching = 256;
                float max_distance = 500;

                //UNITY_LOOP
                for (int i = 1; i <= max_marching; ++i)
                {
                    end+=dir*stepSize;
                    triveled += stepSize;

                    if (triveled > max_distance)
                    return false;

                    float collied = compareWithDepth(end);
                    if (collied>0) //如果碰撞，进行二分查找碰撞点
                    {

						if (collied < thinkness)
						{
							hitUV = ViewPosToCS(end);
							return true;
						}
						//回到当前起点
						end -= dir * stepSize;
						triveled -= stepSize;
						//步进减半
						stepSize *= 0.5;
					}
                }
                return false;
            }

            v2f vert (appdata v)
            {

                v2f o;
                o.vertex =UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                o.positionWS =mul(unity_ObjectToWorld,v.vertex).xyz;
                o.positionOS = v.vertex.xyzw;

                float4 screenPos = ComputeScreenPos(o.vertex);
                screenPos.xyz /= screenPos.w;
                o.positionCS = screenPos;
                //screenPos.xy = screenPos.xy * 0.5 + 0.5;

//#if UNITY_UV_STARTS_AT_TOP
                //o.positionCS.y = 1 - o.positionCS.y;
//#endif

                float zFar = _ProjectionParams.z;
                float4 vsRay = float4(float3(screenPos.xy * 2.0 - 1.0, 1) * zFar, zFar);
                vsRay = mul(unity_CameraInvProjection, vsRay);

                o.vsRay = vsRay;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 screenPos = i.positionCS;
				float depth=SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,screenPos.xy);
				depth = Linear01Depth(depth);

                //float depth = UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,screenPos));
                //depth = Linear01Depth(depth);
               
                float3 wsNormal = normalize(float3(0, 1, 0));    //世界坐标系下的法线
                float3 vsNormal =UnityWorldSpaceViewDir(wsNormal);    //将转换到view space

                float3 vsRayOrigin = i.vsRay * depth;
                //float3 reflectionDir = normalize(reflect(vsRayOrigin, vsNormal));

                float3 viewPosToWorld = normalize(i.positionWS.xyz - _WorldSpaceCameraPos.xyz);
                float3 reflectDir = normalize(reflect(viewPosToWorld, wsNormal));

                float2 hitUV = 0;
                float3 col = 0;
                if (rayMarching(i.positionWS, reflectDir, hitUV))
                {
                    float3 hitCol =tex2D(_GrabTexture, hitUV).xyz;
                    col += hitCol;
                }
                else {
                    
                    col = texCUBE(_SkyBox, reflectDir);
                }

                return float4(col, 1);
            }
            ENDCG
        }
    }
}
