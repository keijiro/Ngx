using UnityEngine;
using System.Collections.Generic;

namespace Ngx
{
    class Feedback : MonoBehaviour
    {
        #region Editable attributes

        [Space]
        [SerializeField, Range(0, 16)] int _modelIndex1 = 0;
        [SerializeField, Range(0, 16)] int _modelIndex2 = 1;
        [SerializeField, Range(0, 1)] float _mixParameter = 0;
        [Space]
        [SerializeField, Range(0, 2)] float _feedbackRate = 1;
        [SerializeField, Range(0, 1)] float _noiseInjection1 = 0;
        [SerializeField, Range(0, 1)] float _noiseInjection2 = 0;
        [SerializeField, Range(0, 1)] float _noiseInjection3 = 0;
        [Space]
        [SerializeField] string [] _modelFiles = null;
        [SerializeField, HideInInspector] Shader _shader = null;

        #endregion

        #region Public properties

        public int ModelIndex1 {
            get { return _modelIndex1; }
            set { _modelIndex1 = value; }
        }

        public int ModelIndex2 {
            get { return _modelIndex2; }
            set { _modelIndex2 = value; }
        }

        public float MixParameter {
            get { return _mixParameter; }
            set { _mixParameter = value; }
        }

        public float FeedbackRate {
            get { return _feedbackRate; }
            set { _feedbackRate = value; }
        }

        public float NoiseInjection1 {
            get { return _noiseInjection1; }
            set { _noiseInjection1 = value; }
        }

        public float NoiseInjection2 {
            get { return _noiseInjection2; }
            set { _noiseInjection2 = value; }
        }

        public float NoiseInjection3 {
            get { return _noiseInjection3; }
            set { _noiseInjection3 = value; }
        }

        #endregion

        #region Internal objects

        Dictionary<string, Pix2Pix.Tensor> [] _models;
        MixGenerator _generator;

        RenderTexture _feedback;
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

            _feedback = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGBHalf);
            _feedback.enableRandomWrite = true;
            _feedback.Create();

            _material = new Material(_shader);
        }

        void OnDestroy()
        {
            foreach (var m in _models) Pix2Pix.WeightReader.DisposeTable(m);
            _generator.Dispose();
            Destroy(_feedback);
            Destroy(_material);
        }

        void Update()
        {
            var tempRT = RenderTexture.GetTemporary(256, 256, 0, _feedback.format);

            // Feedback shader
            _material.SetFloat("_FeedbackRate", _feedbackRate);
            _material.SetVector("_NoiseParams1", new Vector3(0.72f, 5.3f, _noiseInjection1));
            _material.SetVector("_NoiseParams2", new Vector3(33.3f, 3.4f, _noiseInjection2));
            _material.SetVector("_NoiseParams3", new Vector3(62.5f, 2.9f, _noiseInjection3));
            Graphics.Blit(_feedback, tempRT, _material, 0);

            // Pix2Pix generation
            _generator.WeightTable1 = _models[_modelIndex1 % _models.Length];
            _generator.WeightTable2 = _models[_modelIndex2 % _models.Length];
            _generator.MixParameter = _mixParameter;
            _generator.Generate(tempRT, _feedback);
            Pix2Pix.GpuBackend.ExecuteAndClearCommandBuffer();

            RenderTexture.ReleaseTemporary(tempRT);
        }

        void OnRenderObject()
        {
            if ((Camera.current.cullingMask & (1 << gameObject.layer)) == 0) return;
            _material.SetTexture("_MainTex", _feedback);
            _material.SetPass(1);
            Graphics.DrawProcedural(MeshTopology.Triangles, 3, 1);
        }

        #endregion
    }
}
