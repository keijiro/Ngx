using UnityEngine;
using UnityEngine.Events;

namespace Ngx
{
    sealed class NegPosController : MonoBehaviour
    {
        [System.Serializable] class FloatEvent : UnityEvent<float> {}

        [SerializeField] float _bias = 0;
        [SerializeField] FloatEvent _target = new FloatEvent();

        public float NegativeInput
        {
            get { return _negative; }
            set { _negative = value; UpdateValue(); }
        }

        public float PositiveInput
        {
            get { return _positive; }
            set { _positive = value; UpdateValue(); }
        }

        float _negative, _positive;

        void UpdateValue()
        {
            _target.Invoke(_positive - _negative + _bias);
        }
    }
}
