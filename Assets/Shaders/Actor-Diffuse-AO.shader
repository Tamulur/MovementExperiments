Shader "Actor-Diffuse-AO" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_textureSaturation("TextureSaturation", float) = 1
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200

CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
fixed4 _Color;
float _textureSaturation;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed3 gray = fixed3(0.5,0.5,0.5);
	fixed3 black = fixed3(0,0,0);
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Emission = lerp( gray, black, _textureSaturation );
	o.Albedo = lerp(black, c.rgb, _textureSaturation);
	o.Alpha = c.a;
}
ENDCG
}

Fallback "VertexLit"
}
