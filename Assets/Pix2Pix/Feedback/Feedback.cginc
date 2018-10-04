#include "UnityCG.cginc"

sampler2D _MainTex;
sampler2D _FeedbackTex;
half2 _Opacity;

half4 frag(v2f_img i) : SV_Target
{
    half3 c0 = tex2D(_MainTex    , i.uv).rgb * _Opacity.x;
    half3 c1 = tex2D(_FeedbackTex, i.uv).rgb * _Opacity.y;
    half3 c2;

#if defined(BLEND_MULTIPLY)

    c2 = c0 * c1;

#elif defined(BLEND_SCREEN)

    c2 = 1 - (1 - c0) * (1 - c1);

#elif defined(BLEND_SOFTLIGHT)

    half3 a = c0 * c1 * 2 + (1 - c1 * 2) * c0 * c0;
    half3 b = (1 - c1) * c0 * 2 + (c1 * 2 - 1) * sqrt(c0);
    c2 = lerp(a, b, c1 > 0.5);

#else

    half3 a = c0 * c1 * 2;
    half3 b = 1 - (1 - c0) * (1 - c1) * 2;

#if defined(BLEND_OVERLAY)

    c2 = lerp(a, b, c0 > 0.5);

#else // BLEND_HARDLIGHT

    c2 = lerp(a, b, c1 > 0.5);

#endif

#endif

    return half4(c2, 1);
}
