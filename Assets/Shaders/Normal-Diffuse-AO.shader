Shader "Diffuse-AO" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
     _textureSaturation("TextureSaturation", float) = 1
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200

CGPROGRAM
            #pragma surface surf Standard

	sampler2D _MainTex;
	float4 _Goal;
	fixed4 _Color;
	float _textureSaturation;


	half4 LightingStandard (SurfaceOutput s, half3 lightDir, half atten) {
		half NdotL = dot (s.Normal, lightDir);
		half4 c;
		half3 white =half3(0,0,0);
		c.rgb = lerp(white, s.Albedo * _LightColor0.rgb * (NdotL * atten * 2), _textureSaturation);
		c.a = s.Alpha;
		return c;
	}

    inline fixed4 LightingStandard_SingleLightmap (SurfaceOutput s, fixed4 color) {
    	half3 gray = half3(0.5, 0.5, 0.5);
    	half3 black = half3(0, 0, 0);
        half3 lm = lerp(gray, black, _textureSaturation) + DecodeLightmap (color);
        return fixed4(lm, 0);
    }

    inline fixed4 LightingStandard_DualLightmap (SurfaceOutput s, fixed4 totalColor, fixed4 indirectOnlyColor, half indirectFade) {
        half3 lm = lerp (DecodeLightmap (indirectOnlyColor), DecodeLightmap (totalColor), indirectFade);
        return fixed4(lm, 0);
    }

    inline fixed4 LightingStandard_StandardLightmap (SurfaceOutput s, fixed4 color, fixed4 scale, bool surfFuncWritesNormal) {
        UNITY_DIRBASIS

        half3 lm = DecodeLightmap (color);
        half3 scalePerBasisVector = DecodeLightmap (scale);

        if (surfFuncWritesNormal)
        {
            half3 normalInRnmBasis = saturate (mul (unity_DirBasis, s.Normal));
            lm *= dot (normalInRnmBasis, scalePerBasisVector);
        }

        return fixed4(lm, 0);
    }


struct Input {
	float2 uv_MainTex;
	float3 worldPos;
};





void surf (Input IN, inout SurfaceOutput o) {
	fixed4 white = fixed4(1,1,1,1);

	fixed4 c = lerp(white, tex2D(_MainTex, IN.uv_MainTex), _textureSaturation) * _Color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}

ENDCG
}

Fallback "VertexLit"
}
