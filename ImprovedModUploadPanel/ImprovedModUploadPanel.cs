using ImprovedModUploadPanel.Detours;
using UnityEngine;

namespace ImprovedModUploadPanel
{

    public class ImprovedModUploadPanel : MonoBehaviour
    {
        private bool _initialized;

        public static void Initialize()
        {
            if (GameObject.Find("ImprovedModUploadPanel") != null)
            {
                return;
            }
            var go = new GameObject("ImprovedModUploadPanel");
            go.AddComponent<ImprovedModUploadPanel>();
        }

        public void Update()
        {
            if (_initialized)
            {
                return;
            }
            if (!WorkshopModUploadPanelDetour.SetupUI())
            {
                return;
            }          
            _initialized = true;
        }

        public void OnDestroy()
        {
            if (!_initialized)
            {
                return;
            }
            WorkshopModUploadPanelDetour.TearDownUI();
            _initialized = false;
        }
    }
}
