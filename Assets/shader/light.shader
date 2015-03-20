Shader "Custom/light" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader {
		Tags { "Queue" = "Transparent+1" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 200
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Always
		Fog {Mode Off}
		
		Pass {
			Blend One One
			CGPROGRAM
			#pragma vertex vert 
		    #pragma fragment frag
		    #include "UnityCG.cginc"
		    struct VertIn {
	        	float4 vertex : POSITION;
	        	float4 texcoord : TEXCOORD0;
	        };
	        struct v2f {
	        	fixed4 pos : SV_POSITION;
	        	fixed2 uv : TEXCOORD0;
	   		};
	        uniform sampler2D _MainTex;
	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				
				return o;
			}
			fixed4 frag(v2f i) : Color {
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
	        
		    ENDCG
		}
		
	} 
	FallBack "Diffuse"
}
