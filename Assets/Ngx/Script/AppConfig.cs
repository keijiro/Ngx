using UnityEngine;

namespace Ngx
{
    sealed class AppConfig : MonoBehaviour
    {
        void Start()
        {
            #if !UNITY_EDITOR
            Cursor.visible = false;
            #endif
        }
    }
}
