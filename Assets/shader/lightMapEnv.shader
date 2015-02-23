Shader "Custom/lightMapEnv" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		//_Color ("Ambient Color", Color) = (0.588, 0.588, 0.588, 1)
		
		//_LightMap ("Light Map ", 2D) = "white" {}
		//_CamPos ("Camera pos", Vector) = (0, 0, 0, 0)
		//_CameraSize ("Camera Size", float) = 10
		
		//_LightCoff ("light Cofficient", float) = 2
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
	
		Pass {	
			LOD 200
			Lighting Off
			
			
			CGPROGRAM
			#pragma vertex vert 
		    #pragma fragment frag
		    #include "UnityCG.cginc"
		        
	        struct VertIn {
	        	float4 vertex : POSITION;
	        	float4 texcoord : TEXCOORD0;
	        	//float4 color : COLOR;
	        	
	        };

			
			struct v2f {
	        	fixed4 pos : SV_POSITION;
	        	fixed2 uv : TEXCOORD0;
	   			//fixed4 vertColor : TEXCOORD1;
	   			fixed3 offPos : TEXCOORD2;
	        };
			uniform sampler2D _MainTex;
			//uniform fixed4 _Color;
			
			uniform sampler2D _LightMap;
		    uniform float4 _CamPos;
		    uniform float _CameraSize;
			//uniform fixed _LightCoff;
			
			v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				
				//o.vertColor = v.color; 
				o.offPos = mul(_Object2World, v.vertex).xyz-(_WorldSpaceCameraPos+_CamPos);
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
				//
	        	//fixed4 col =  tex2D(_MainTex, i.uv)*((UNITY_LIGHTMODEL_AMBIENT)/1.5+tex2D(_LightMap, (i.offPos.xz+float2(_CameraSize, _CameraSize))/(2*_CameraSize))*2 );
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}	

			ENDCG
		}
	} 
	FallBack "Diffuse"
}
