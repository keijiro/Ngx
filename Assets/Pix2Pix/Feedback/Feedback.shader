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
            #define BLEND_MULTIPLY
            #include "Feedback.cginc"
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #define BLEND_SCREEN
            #include "Feedback.cginc"
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #define BLEND_OVERLAY
            #include "Feedback.cginc"
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #define BLEND_HARDLIGHT
            #include "Feedback.cginc"
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #define BLEND_SOFTLIGHT
            #include "Feedback.cginc"
            ENDCG
        }
    }
}
