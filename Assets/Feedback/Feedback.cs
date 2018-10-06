using UnityEngine;
using System.Collections.Generic;

namespace Pix2Pix
{
    class Feedback : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] string [] _weightFileNames = null;
        [SerializeField, HideInInspector] Shader _shader = null;

        #endregion

        #region Internal objects

        Dictionary<string, Pix2Pix.Tensor> [] _weights;
        Pix2Pix.Generator _generator;
        RenderTexture _feedbackRT;
        Material _material;

        #endregion

        #region MonoBehaviour implementation

        void Start()
        {
            _weights = new Dictionary<string, Pix2Pix.Tensor>[_weightFileNames.Length];

            for (var i = 0; i < _weightFileNames.Length; i++)
                _weights[i] = Pix2Pix.WeightReader.ReadFromFile(
                    System.IO.Path.Combine(
                        Application.streamingAssetsPath, _weightFileNames[i]));

            _generator = new Pix2Pix.Generator();

            _feedbackRT = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGBFloat);
            _feedbackRT.enableRandomWrite = true;
            _feedbackRT.Create();

            _material = new Material(_shader);
        }

        void OnDestroy()
        {
            foreach (var w in _weights) Pix2Pix.WeightReader.DisposeTable(w);
            _generator.Dispose();
            Destroy(_feedbackRT);
            Destroy(_material);
        }

        void Update()
        {
            var select = (Time.frameCount / 30) % _weights.Length;
            _generator.WeightTable = _weights[select];

            _generator.Start(_feedbackRT);
            while (true)
            {
                _generator.Step();
                if (!_generator.Running) break;
            }
            _generator.GetResult(_feedbackRT);

            Pix2Pix.GpuBackend.ExecuteAndClearCommandBuffer();
        }

        void OnRenderObject()
        {
            if ((Camera.current.cullingMask & (1 << gameObject.layer)) == 0) return;
            _material.SetTexture("_MainTex", _feedbackRT);
            _material.SetPass(0);
            Graphics.DrawProcedural(MeshTopology.Triangles, 3, 1);
        }

        #endregion
    }
}
