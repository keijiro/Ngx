using UnityEngine;
using UnityEngine.Events;
using MidiJack;

namespace Ngx
{
    sealed class MidiNoteEventReceiver : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] int _note = 48;
        [SerializeField] UnityEvent _noteOnEvent = new UnityEvent();
        [SerializeField] UnityEvent _noteOffEvent = new UnityEvent();

        #endregion

        #region MIDI event delegates

        void NoteOnEvent(MidiChannel channel, int note, float velocity)
        {
            if (note == _note) _noteOnEvent.Invoke();
        }

        void NoteOffEvent(MidiChannel channel, int note)
        {
            if (note == _note) _noteOffEvent.Invoke();
        }

        #endregion

        #region MonoBehaviour implementation

        void OnEnable()
        {
            MidiMaster.noteOnDelegate += NoteOnEvent;
            MidiMaster.noteOffDelegate += NoteOffEvent;
        }

        void OnDisable()
        {
            MidiMaster.noteOnDelegate -= NoteOnEvent;
            MidiMaster.noteOffDelegate -= NoteOffEvent;
        }

        #endregion
    }
}
