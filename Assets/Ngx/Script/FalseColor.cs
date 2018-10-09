using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Unity.Mathematics;

namespace Ngx
{
    #region Effect settings

    [System.Serializable]
    [PostProcess(typeof(FalseColorRenderer), PostProcessEvent.AfterStack, "Ngx/FalseColor")]
    public sealed class FalseColor : PostProcessEffectSettings
    {
        public IntParameter randomSeed = new IntParameter{ value = 1 };
        [Range(0, 1)] public FloatParameter strength = new FloatParameter();
    }

    #endregion

    #region Effect renderer

    sealed class FalseColorRenderer : PostProcessEffectRenderer<FalseColor>
    {
        static class ShaderIDs
        {
            internal static readonly int ParamFrq = Shader.PropertyToID("_ParamFrq");
            internal static readonly int ParamSpd = Shader.PropertyToID("_ParamSpd");
            internal static readonly int ParamAmp = Shader.PropertyToID("_ParamAmp");
            internal static readonly int ParamOfs = Shader.PropertyToID("_ParamOfs");
            internal static readonly int Strength = Shader.PropertyToID("_Strength");
        }

        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(Shader.Find("Hidden/Ngx/FalseColor"));

            var seed = (uint)(settings.randomSeed + 0x10000000);
            var rand = new Unity.Mathematics.Random(seed);

            var frq = rand.NextFloat3((float)math.PI, (float)math.PI * 12);
            var spd = rand.NextFloat3(-2, 2);
            var amp = rand.NextFloat3(1);
            var ofs = rand.NextFloat3(1);

            sheet.properties.SetVector(ShaderIDs.ParamFrq, (Vector3)frq);
            sheet.properties.SetVector(ShaderIDs.ParamSpd, (Vector3)spd);
            sheet.properties.SetVector(ShaderIDs.ParamAmp, (Vector3)amp);
            sheet.properties.SetVector(ShaderIDs.ParamOfs, (Vector3)ofs);
            sheet.properties.SetFloat(ShaderIDs.Strength, settings.strength);

            var cmd = context.command;
            cmd.BeginSample("FalseColor");
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
            cmd.EndSample("FalseColor");
        }
    }

    #endregion
}
