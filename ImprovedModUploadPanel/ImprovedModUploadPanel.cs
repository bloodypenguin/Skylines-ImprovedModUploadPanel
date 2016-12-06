using ImprovedModUploadPanel.Detours;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ImprovedModUploadPanel
{

    public class ImprovedModUploadPanel
    {
        public static void Initialize()
        {
            if (GameObject.Find("ImprovedModUploadPanel") != null)
            {
                return;
            }
            WorkshopModUploadPanelDetour.Deploy();
            var go = new GameObject("ImprovedModUploadPanel");
            var updateHook = go.AddComponent<UpdateHook>();
            updateHook.once = false;
            updateHook.onUnityUpdate = () =>
            {
                if (!WorkshopModUploadPanelDetour.SetupUI())
                {
                    return;
                }
                PackageEntryDetour.Initialize();
                updateHook.once = true;
            };
        }

        public static void Revert()
        {
            var go = GameObject.Find("ImprovedModUploadPanel");
            if (go == null)
            {
                return;
            }
            PackageEntryDetour.Revert();
            WorkshopModUploadPanelDetour.Revert();
            WorkshopModUploadPanelDetour.TearDownUI();
            Object.DestroyImmediate(go);
        }
    }
}
