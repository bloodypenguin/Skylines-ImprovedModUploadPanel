using ICities;
using ImprovedModUploadPanel.Detours;

namespace ImprovedModUploadPanel
{
    public class Mod : IUserMod
    {
        public static bool bootstrapped;

        public Mod()
        {
            if (!bootstrapped)
            {
                WorkshopModUploadPanelDetour.Deploy();
                PackageEntryDetour.Deploy();
                bootstrapped = true;
            }
        }

        public string Name
        {
            get
            {
                ImprovedModUploadPanel.Initialize();
                return "Improved Mod Upload Panel";
            }
        }

        public string Description => "Improved Mod Upload Panel";
    }
}
