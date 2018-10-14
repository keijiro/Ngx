using UnityEngine;

namespace Ngx
{
    sealed class AppController : MonoBehaviour
    {
        [SerializeField] GameObject _controllerUI = null;
        [SerializeField] UnityEngine.UI.Text _fpsDisplay = null;

        float _fpsCountStart = 0;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                _controllerUI.SetActive(!_controllerUI.activeSelf);
                Cursor.visible = !Cursor.visible;
            }

            if (Time.frameCount % 10 == 0)
            {
                var current = Time.time;
                var fps = 10 /  (current - _fpsCountStart);
                _fpsDisplay.text = fps.ToString("FPS: 0.0");
                _fpsCountStart = current;
            }
        }
    }
}
