using UnityEngine;
using System.Collections.Generic;
using Pix2Pix;

namespace Pix2PixFeedback
{
    public class MixGenerator : System.IDisposable
    {
        #region Constructor and IDisposable implementation

        public MixGenerator()
        {
            // Temporary tensors
            for (var i = 0; i < 8; i ++) _skip[i] = new Tensor();
            _temp1 = new Tensor();
            _temp2 = new Tensor();
            _temp3 = new Tensor();
            _temp4 = new Tensor();

            // Keys for weight table
            _encoders[0] = new Layer {
                Kernel = "generator/encoder_1/conv2d/kernel",
                Bias   = "generator/encoder_1/conv2d/bias"
            };

            _decoders[0] = new Layer {
                Kernel = "generator/decoder_1/conv2d_transpose/kernel",
                Bias   = "generator/decoder_1/conv2d_transpose/bias"
            };

            for (var i = 1; i < 8; i++)
            {
                var scope = "generator/encoder_" + (i + 1);

                _encoders[i] = new Layer {
                    Kernel = scope + "/conv2d/kernel",
                    Bias   = scope + "/conv2d/bias",
                    Beta   = scope + "/batch_normalization/beta",
                    Gamma  = scope + "/batch_normalization/gamma"
                };

                scope = "generator/decoder_" + (i + 1);

                _decoders[i] = new Layer {
                    Kernel = scope + "/conv2d_transpose/kernel",
                    Bias   = scope + "/conv2d_transpose/bias",
                    Beta   = scope + "/batch_normalization/beta",
                    Gamma  = scope + "/batch_normalization/gamma"
                };
            }
        }

        public void Dispose()
        {
            foreach (var t in _skip) t.Dispose();
            _temp1.Dispose();
            _temp2.Dispose();
            _temp3.Dispose();
            _temp4.Dispose();
        }

        #endregion

        #region Internal structure

        // Encoder/decoder layers
        struct Layer { public string Kernel, Bias, Beta, Gamma; }
        Layer[] _encoders = new Layer[8];
        Layer[] _decoders = new Layer[8];

        // Temporary tensors
        Tensor[] _skip = new Tensor[8];
        Tensor _temp1, _temp2, _temp3, _temp4;

        #endregion

        #region Public properties and methods

        public Dictionary<string, Tensor> WeightTable1 { get; set; }
        public Dictionary<string, Tensor> WeightTable2 { get; set; }
        public float MixParameter { get; set; }

        public void Generate(Texture input, RenderTexture output)
        {
            var w1 = WeightTable1;
            var w2 = WeightTable2;

            Image.ConvertToTensor(input, _temp1);

            {
                var l = _encoders[0];
                Math.Blend(w1[l.Kernel], w2[l.Kernel], MixParameter, _temp3);
                Math.Blend(w1[l.Bias  ], w2[l.Bias  ], MixParameter, _temp4);
                Math.Conv2D(_temp1, _temp3, _temp4, _skip[0]);
            }

            for (var i = 1; i < 8; i++)
            {
                var l = _encoders[i];
                Math.LeakyRelu(_skip[i - 1], 0.2f, _temp1);
                Math.Blend(w1[l.Kernel], w2[l.Kernel], MixParameter, _temp3);
                Math.Blend(w1[l.Bias  ], w2[l.Bias  ], MixParameter, _temp4);
                Math.Conv2D(_temp1, _temp3, _temp4, _temp2);
                Math.Blend(w1[l.Gamma], w2[l.Gamma], MixParameter, _temp3);
                Math.Blend(w1[l.Beta ], w2[l.Beta ], MixParameter, _temp4);
                Math.BatchNorm(_temp2, _temp3, _temp4, _skip[i]);
            }

            {
                var l = _decoders[7];
                Math.Relu(_skip[7], _temp1);
                Math.Blend(w1[l.Kernel], w2[l.Kernel], MixParameter, _temp3);
                Math.Blend(w1[l.Bias  ], w2[l.Bias  ], MixParameter, _temp4);
                Math.Deconv2D(_temp1, _temp3, _temp4, _temp2);
                Math.Blend(w1[l.Gamma], w2[l.Gamma], MixParameter, _temp3);
                Math.Blend(w1[l.Beta ], w2[l.Beta ], MixParameter, _temp4);
                Math.BatchNorm(_temp2, _temp3, _temp4, _temp1);
            }

            for (var i = 6; i > 0; i--)
            {
                var l = _decoders[i];
                Math.Concat(_temp1, _skip[i], _temp2);
                Math.Relu(_temp2, _temp1);
                Math.Blend(w1[l.Kernel], w2[l.Kernel], MixParameter, _temp3);
                Math.Blend(w1[l.Bias  ], w2[l.Bias  ], MixParameter, _temp4);
                Math.Deconv2D(_temp1, _temp3, _temp4, _temp2);
                Math.Blend(w1[l.Gamma], w2[l.Gamma], MixParameter, _temp3);
                Math.Blend(w1[l.Beta ], w2[l.Beta ], MixParameter, _temp4);
                Math.BatchNorm(_temp2, _temp3, _temp4, _temp1);
            }

            {
                var l = _decoders[0];
                Math.Concat(_temp1, _skip[0], _temp2);
                Math.Relu(_temp2, _temp1);
                Math.Blend(w1[l.Kernel], w2[l.Kernel], MixParameter, _temp3);
                Math.Blend(w1[l.Bias  ], w2[l.Bias  ], MixParameter, _temp4);
                Math.Deconv2D(_temp1, _temp3, _temp4, _temp2);
                Math.Tanh(_temp2, _temp1);
            }

            Image.ConvertFromTensor(_temp1, output);
        }

        #endregion
    }
}

