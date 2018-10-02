Shader "Hidden/Pix2Pix/Feedback"
{
    Properties
    {
        _MainTex("", 2D) = "" {}
        _FeedbackTex("", 2D) = "" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM

            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _FeedbackTex;
            fixed2 _Opacity;

            fixed4 frag(v2f_img i) : SV_Target
            {
                fixed4 c0 = tex2D(_MainTex, i.uv);
                fixed4 c1 = tex2D(_FeedbackTex, i.uv);
                return c0 * _Opacity.x + c1 *_Opacity.y;
            }

            ENDCG
        }
    }
}
