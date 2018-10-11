Shader "Hidden/Ngx/Feedback"
{
    Properties
    {
        _MainTex("", 2D) = "" {}
        _BlendTex("", 2D) = "" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"
    #include "SimplexNoise3D.hlsl"

    sampler2D _MainTex;

    half _FeedbackRate;
    half3 _NoiseParams1; // frequency, speed, amplitude
    half3 _NoiseParams2;
    half3 _NoiseParams3;

    half4 FragmentInject(v2f_img i) : SV_Target
    {
        half4 c = tex2D(_MainTex, i.uv) * _FeedbackRate;

        float3 np = float3(i.uv, _Time.y);
        half n1 = snoise(np * _NoiseParams1.xxy) * _NoiseParams1.z;
        half n2 = snoise(np * _NoiseParams2.xxy) * _NoiseParams2.z;
        half n3 = snoise(np * _NoiseParams3.xxy) * _NoiseParams3.z;

        return clamp(c + n1 + n2 + n3, -100, 100);
    }

    void VertexBlit(
        uint vid : SV_VertexID,
        out float4 position : SV_Position,
        out float2 texcoord : TEXCOORD
    )
    {
        float x = vid > 1;
        float y = vid > 0;

        position = float4(x * 4 - 1, y * 4 - 3, 1, 1);
        texcoord = float2(x * 2 - 0, 2 - y * 2);
    }

    half4 FragmentBlit(
        float4 position : SV_Position,
        float2 texcoord : TEXCOORD
    ) : SV_Target
    {
        return tex2D(_MainTex, texcoord);
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment FragmentInject
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex VertexBlit
            #pragma fragment FragmentBlit
            ENDCG
        }
    }
}
