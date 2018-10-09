using UnityEngine;
using UnityEngine.Events;
using MidiJack;

namespace Ngx
{
    sealed class MidiKnobEventReceiver : MonoBehaviour
    {
        #region Editable attributes

        [System.Serializable] class FloatEvent : UnityEvent<float> {}

        [SerializeField] int _controlID = 10;
        [SerializeField] FloatEvent _knobEvent = new FloatEvent();

        #endregion

        #region MIDI event delegates

        void KnobEvent(MidiChannel channel, int id, float value)
        {
            if (id == _controlID) _knobEvent.Invoke(value);
        }

        #endregion

        #region MonoBehaviour implementation

        void OnEnable()
        {
            MidiMaster.knobDelegate += KnobEvent;
        }

        void OnDisable()
        {
            MidiMaster.knobDelegate -= KnobEvent;
        }

        #endregion
    }
}
