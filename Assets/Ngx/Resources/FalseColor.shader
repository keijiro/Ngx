Shader "Hidden/Ngx/FalseColor"
{
    HLSLINCLUDE

    #include "PostProcessing/Shaders/StdLib.hlsl"
    #include "PostProcessing/Shaders/Colors.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

    half3 _ParamFrq;
    half3 _ParamSpd;
    half3 _ParamAmp;
    half3 _ParamOfs;

    half _Strength;

    half4 Frag(VaryingsDefault i) : SV_Target
    {
        half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

    #ifndef UNITY_COLORSPACE_GAMMA
        c.rgb = LinearToSRGB(c.rgb);
    #endif

        half3 fc = sin(c.rgb * _ParamFrq + _Time.y * _ParamSpd);
        fc = fc * _ParamAmp + _ParamOfs;

        c.rgb = lerp(c.rgb, saturate(fc), _Strength);

    #ifndef UNITY_COLORSPACE_GAMMA
        c.rgb = SRGBToLinear(c.rgb);
    #endif

        return c;
    }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA
            ENDHLSL
        }
    }
}
