using UnityEngine;

namespace Ngx
{
    sealed class AppController : MonoBehaviour
    {
        [SerializeField] GameObject _controllerUI = null;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                _controllerUI.SetActive(!_controllerUI.activeSelf);
                Cursor.visible = !Cursor.visible;
            }
        }
    }
}
