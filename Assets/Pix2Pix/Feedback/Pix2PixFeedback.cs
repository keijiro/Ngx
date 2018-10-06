using UnityEngine;
using System.Collections.Generic;

namespace Pix2Pix
{
    [ImageEffectOpaque]
    class Pix2PixFeedback : MonoBehaviour
    {
        public enum BlendMode { Multiply, Screen, Overlay, HardLight, SoftLight }

        [SerializeField] string [] _weightFileNames = null;
        [SerializeField] BlendMode _blendMode = BlendMode.Multiply;

        [SerializeField, Range(0, 1)] float _injection = 1;
        [SerializeField, Range(0, 1)] float _feedback = 1;

        [SerializeField, HideInInspector] Shader _shader = null;

        Dictionary<string, Pix2Pix.Tensor> [] _weights;

        Pix2Pix.Generator _generator;

        RenderTexture _feedbackRT, _temporaryRT;

        Material _material;

        void Start()
        {
            _weights = new Dictionary<string, Pix2Pix.Tensor>[_weightFileNames.Length];

            for (var i = 0; i < _weightFileNames.Length; i++)
                _weights[i] = Pix2Pix.WeightReader.ReadFromFile
                    (System.IO.Path.Combine(Application.streamingAssetsPath, _weightFileNames[i]));

            _generator = new Pix2Pix.Generator();

            _feedbackRT = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGBFloat);
            _feedbackRT.filterMode = FilterMode.Point;
            _feedbackRT.Create();

            _temporaryRT = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGBFloat);
            _temporaryRT.filterMode = FilterMode.Point;
            _temporaryRT.enableRandomWrite = true;
            _temporaryRT.Create();

            _material = new Material(_shader);
        }

        void OnDestroy()
        {
            _generator.Dispose();

            foreach (var w in _weights) Pix2Pix.WeightReader.DisposeTable(w);

            Destroy(_feedbackRT);
            Destroy(_temporaryRT);
            Destroy(_material);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            _generator.WeightTable =
                _weights[(Time.frameCount / 30) % _weights.Length];

            _generator.Start(_feedbackRT);
            while (true)
            {
                _generator.Step();
                if (!_generator.Running) break;
            }
            _generator.GetResult(_temporaryRT);

            Pix2Pix.GpuBackend.ExecuteAndClearCommandBuffer();

            _material.SetTexture("_FeedbackTex", _temporaryRT);
            _material.SetVector("_Opacity", new Vector2(_injection, _feedback));
            Graphics.Blit(source, _feedbackRT, _material, (int)_blendMode);

            Graphics.Blit(_feedbackRT, destination);
        }
    }
}
