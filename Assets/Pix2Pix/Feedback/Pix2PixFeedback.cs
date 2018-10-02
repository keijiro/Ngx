using UnityEngine;
using System.Collections.Generic;

namespace Pix2Pix
{
    class Pix2PixFeedback : MonoBehaviour
    {
        [SerializeField] string _weightFileName = null;

        [SerializeField, Range(0, 1)] float _injection = 1;
        [SerializeField, Range(0, 1)] float _feedback = 1;

        [SerializeField, HideInInspector] Shader _shader = null;

        Dictionary<string, Pix2Pix.Tensor> _weightTable;
        Pix2Pix.Generator _generator;

        RenderTexture _feedbackRT, _temporaryRT;

        Material _material;

        void Start()
        {
            var filePath = System.IO.Path.Combine(Application.streamingAssetsPath, _weightFileName);
            _weightTable = Pix2Pix.WeightReader.ReadFromFile(filePath);
            _generator = new Pix2Pix.Generator(_weightTable);

            _feedbackRT = new RenderTexture(256, 256, 0);
            _feedbackRT.Create();

            _temporaryRT = new RenderTexture(256, 256, 0);
            _temporaryRT.enableRandomWrite = true;
            _temporaryRT.Create();

            _material = new Material(_shader);
        }

        void OnDestroy()
        {
            _generator.Dispose();
            Pix2Pix.WeightReader.DisposeTable(_weightTable);

            Destroy(_feedbackRT);
            Destroy(_temporaryRT);
            Destroy(_material);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
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
            Graphics.Blit(source, _feedbackRT, _material);

            Graphics.Blit(_feedbackRT, destination);
        }
    }
}
