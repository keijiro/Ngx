using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Unity.Mathematics;

namespace Ngx
{
    #region Effect settings

    [System.Serializable]
    [PostProcess(typeof(ColorMixerRenderer), PostProcessEvent.AfterStack, "Ngx/Color Mixer")]
    public sealed class ColorMixer : PostProcessEffectSettings
    {
        [Range(0, 1)] public FloatParameter intensity = new FloatParameter();
        public IntParameter randomSeed = new IntParameter();
    }

    #endregion

    #region Effect renderer

    sealed class ColorMixerRenderer : PostProcessEffectRenderer<ColorMixer>
    {
        static class ShaderIDs
        {
            internal static readonly int Intensity = Shader.PropertyToID("_Intensity");
            internal static readonly int MixerMatrix = Shader.PropertyToID("_MixerMatrix");
        }

        Unity.Mathematics.Random _random;

        public override void Render(PostProcessRenderContext context)
        {
            _random.InitState((uint)Mathf.Max(1, settings.randomSeed));

            var ri = _random.NextInt3(-1, 2);
            var gi = _random.NextInt3(-1, 2);
            var bi = _random.NextInt3(-1, 2);

            var rf = new float4(ri, 0);
            var gf = new float4(gi, 0);
            var bf = new float4(bi, 0);

            var af = new float4(0, 0, 0, 1);
            af -= math.min(rf, 0);
            af -= math.min(gf, 0);
            af -= math.min(bf, 0);

            var cm = new float4x4(rf, gf, bf, af);

            var sheet = context.propertySheets.Get(Shader.Find("Hidden/Ngx/ColorMixer"));
            sheet.properties.SetFloat(ShaderIDs.Intensity, settings.intensity);
            sheet.properties.SetMatrix(ShaderIDs.MixerMatrix, (Matrix4x4)cm);

            var cmd = context.command;
            cmd.BeginSample("Color mixer");
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
            cmd.EndSample("Color mixer");
        }
    }

    #endregion
}
