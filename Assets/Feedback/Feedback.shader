Shader "Hidden/Pix2Pix/Feedback"
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

    half4 _FeedbackParams; // feedback rate, noise frequency/speed/strength

    half4 FragmentInject(v2f_img i) : SV_Target
    {
        half4 c = tex2D(_MainTex, i.uv) * _FeedbackParams.x;
        half n = snoise(float3(i.uv, _Time.y) * _FeedbackParams.yyz);
        return saturate(c + n * _FeedbackParams.w);
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
