// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Modifiable Diffuse Shader"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        _TransX ("Trans X", float) = 0
        _TransY ("Trans Y", float) = 0
        _Scale ("Scale", float) = 1
        _Rotation ("Rotation", float) = 0
        _RotationOffset("Rotation Offset", float) = 0
    }
    SubShader
    {
        Pass
        {
            Tags {"LightMode"="ForwardBase"}
            Lighting On
        	Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            // compile shader into multiple variants, with and without shadows
            // (we don't care about any lightmaps yet, so skip these variants)
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            // shadow helper functions and macros
            #include "AutoLight.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                SHADOW_COORDS(1) // put shadows data into TEXCOORD1
                fixed3 diff : COLOR0;
                fixed3 ambient : COLOR1;
                float4 pos : SV_POSITION;
            };

            float4x4 _TextureRotation;
            float _Rotation, _RotationOffset, _TransX, _TransY, _Scale;

            v2f vert (appdata_base v)
            {

            	float sinF = sin(radians(_Rotation));
				float cosF = cos(radians(_Rotation));
				float tx = _TransX;
				float ty = _TransY;
				_TransX = (cosF * tx) - (sinF * ty);
				_TransY = (sinF * tx) + (cosF * ty);

            	float4x4 translateMatrix = float4x4(1,	0,	0,	_TransX,
											 		0,	1,	0,	_TransY,
									  				0,	0,	1,	0,
									  				0,	0,	0,	1);
	
				float4x4 scaleMatrix 	= float4x4(_Scale,	0,	0,	0,
											 		0,	_Scale,0,	0,
								  					0,	0,	_Scale, 0,
								  					0,	0,	0,	    1);
            	float angleZ = radians(_Rotation+_RotationOffset);
				float c = cos(angleZ);
				float s = sin(angleZ);
				float4x4 rotateZMatrix	= float4x4(	c,	-s,	0,	0,
											 		s,	c,	0,	0,
								  					0,	0,	1,	0,
								  					0,	0,	0,	1);

				float4x4 localTranslated = translateMatrix;
  				float4x4 localScaledTranslated = mul(localTranslated,scaleMatrix);
  				float4x4 localScaledTranslatedRotZ = mul(localScaledTranslated,rotateZMatrix);

				_TextureRotation = localScaledTranslatedRotZ;



                v2f o;
//                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;

                o.pos = UnityObjectToClipPos(v.vertex);
//                o.uv = mul(_TextureRotation, float4(v.texcoord,0,1)).xy;
                o.uv = mul(_TextureRotation, v.texcoord).xy;


                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0.rgb;
                o.ambient = ShadeSH9(half4(worldNormal,1));
                // compute shadows data
                TRANSFER_SHADOW(o)
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // compute shadow attenuation (1.0 = fully lit, 0.0 = fully shadowed)
                fixed shadow = SHADOW_ATTENUATION(i);
                // darken light's illumination with shadow, keep ambient intact
                fixed3 lighting = i.diff * shadow + i.ambient;
                col.rgb *= lighting;
                return col;
            }
            ENDCG
        }

        // shadow casting support
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}