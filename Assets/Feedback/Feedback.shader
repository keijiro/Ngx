Shader "Hidden/Pix2Pix/Feedback"
{
    Properties
    {
        _MainTex("", 2D) = "" {}
        _BlendTex("", 2D) = "" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    sampler2D _BlendTex;
    half2 _BlendParams;

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

    half4 FragmentBlend(v2f_img i) : SV_Target
    {
        half4 c1 = tex2D(_MainTex, i.uv) * _BlendParams.x;
        half4 c2 = tex2D(_BlendTex, i.uv) * _BlendParams.y;
        return saturate(c1 + c2);
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex VertexBlit
            #pragma fragment FragmentBlit
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment FragmentBlend
            ENDCG
        }
    }
}
