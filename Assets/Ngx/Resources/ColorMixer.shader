Shader "Hidden/Ngx/ColorMixer"
{
    HLSLINCLUDE

    #include "PostProcessing/Shaders/StdLib.hlsl"
    #include "PostProcessing/Shaders/Colors.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

    half4x4 _MixerMatrix;
    half _Intensity;

    half4 FragColorMixer(VaryingsDefault input) : SV_Target
    {
        half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.texcoord);
        return lerp(c, mul(_MixerMatrix, half4(c.xyz, 1)), _Intensity);
    }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment FragColorMixer
            ENDHLSL
        }
    }
}
