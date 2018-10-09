using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Ngx
{
    [RequireComponent(typeof(PostProcessVolume))]
    sealed class VolumeAdapter : MonoBehaviour
    {
        public float Weight
        {
            get { return _volume.weight; }
            set { _volume.weight = value; }
        }

        PostProcessVolume _volume;

        void Start()
        {
            _volume = GetComponent<PostProcessVolume>();
        }
    }
}
