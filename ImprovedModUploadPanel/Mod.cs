using ICities;
using ImprovedModUploadPanel.Detours;
using ImprovedModUploadPanel.Redirection;

namespace ImprovedModUploadPanel
{
    public class Mod : IUserMod
    {

        public Mod()
        {
            Redirector<WorkshopModUploadPanelDetour>.Deploy();
            Redirector<PackageEntryDetour>.Deploy();
        }

        public string Name => "Improved Mod Upload Panel";

        public string Description => "Improved Mod Upload Panel";
    }
}
