using UnityEngine;
using System.Collections.Generic;

namespace Pix2PixFeedback
{
    class Feedback : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField, Range(0, 1)] float _mixParameter = 0;
        [SerializeField, Range(0, 2)] float _feedbackRate = 1;
        [SerializeField, Range(0, 1)] float _noiseInjection = 0;
        [SerializeField, Range(0, 16)] int _modelIndex1 = 0;
        [SerializeField, Range(0, 16)] int _modelIndex2 = 1;
        [SerializeField] string [] _modelFiles = null;
        [SerializeField, HideInInspector] Shader _shader = null;

        #endregion

        #region Internal objects

        Dictionary<string, Pix2Pix.Tensor> [] _models;
        MixGenerator _generator;

        RenderTexture _tempRT, _delayRT, _backRT;
        Material _material;

        string GetModelFilePath(int index)
        {
            var file = _modelFiles[index] + ".pict";
            return System.IO.Path.Combine(Application.streamingAssetsPath, file);
        }

        #endregion

        #region MonoBehaviour implementation

        void Start()
        {
            _models = new Dictionary<string, Pix2Pix.Tensor>[_modelFiles.Length];

            for (var i = 0; i < _modelFiles.Length; i++)
                _models[i] = Pix2Pix.WeightReader.ReadFromFile(GetModelFilePath(i));

            _generator = new MixGenerator();

            _tempRT  = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGBFloat);
            _delayRT = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGBFloat);
            _backRT  = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGBFloat);

            _tempRT.enableRandomWrite = true;

            _tempRT .Create();
            _delayRT.Create();
            _backRT .Create();

            _material = new Material(_shader);
        }

        void OnDestroy()
        {
            foreach (var m in _models) Pix2Pix.WeightReader.DisposeTable(m);

            _generator.Dispose();

            Destroy(_tempRT);
            Destroy(_delayRT);
            Destroy(_backRT);

            Destroy(_material);
        }

        void Update()
        {
            // Blending parameters
            var blendParams = new Vector3(_noiseInjection, 0, 1);
            blendParams *= _feedbackRate;
            _material.SetVector("_BlendParams", blendParams);

            // External noise injection
            Graphics.Blit(_delayRT, _tempRT, _material, 0);

            // Set the currently selected model to the generator.
            _generator.WeightTable1 = _models[_modelIndex1 % _models.Length];
            _generator.WeightTable2 = _models[_modelIndex2 % _models.Length];

            // Pix2Pix generation
            _generator.MixParameter = _mixParameter;
            _generator.Generate(_tempRT, _tempRT);
            Pix2Pix.GpuBackend.ExecuteAndClearCommandBuffer();

            // Blend with the previous frame.
            _material.SetTexture("_BlendTex", _tempRT);
            Graphics.Blit(_delayRT, _backRT, _material, 1);

            var rt = _backRT;
            _backRT = _delayRT;
            _delayRT = rt;
        }

        void OnRenderObject()
        {
            if ((Camera.current.cullingMask & (1 << gameObject.layer)) == 0) return;
            _material.SetTexture("_MainTex", _delayRT);
            _material.SetPass(2);
            Graphics.DrawProcedural(MeshTopology.Triangles, 3, 1);
        }

        #endregion
    }
}
