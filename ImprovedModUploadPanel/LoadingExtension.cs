using ICities;

namespace ImprovedModUploadPanel
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            ImprovedModUploadPanel.Revert();
        }

        public override void OnReleased()
        {
            base.OnReleased();
            ImprovedModUploadPanel.Initialize();
        }
    }
}