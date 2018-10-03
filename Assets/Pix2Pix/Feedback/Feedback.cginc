#include "UnityCG.cginc"

sampler2D _MainTex;
sampler2D _FeedbackTex;
half2 _Opacity;

half4 frag(v2f_img i) : SV_Target
{
    half4 c0 = tex2D(_MainTex    , i.uv) * _Opacity.x;
    half4 c1 = tex2D(_FeedbackTex, i.uv) * _Opacity.y;

#if defined(BLEND_MULTIPLY)

    return c0 * c1;

#elif defined(BLEND_SCREEN)

    return 1 - (1 - c0) * (1 - c1);

#elif defined(BLEND_SOFTLIGHT)

    half4 a = c0 * c1 * 2 + (1 - c1 * 2) * c0 * c0;
    half4 b = (1 - c1) * c0 * 2 + (c1 * 2 - 1) * sqrt(c0);
    return lerp(a, b, c1 > 0.5);

#else

    half4 a = c0 * c1 * 2;
    half4 b = 1 - (1 - c0) * (1 - c1) * 2;

#if defined(BLEND_OVERLAY)

    return lerp(a, b, c0 > 0.5);

#else // BLEND_HARDLIGHT

    return lerp(a, b, c1 > 0.5);

#endif

#endif
}
