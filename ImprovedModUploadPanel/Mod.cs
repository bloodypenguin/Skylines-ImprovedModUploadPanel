using ICities;

namespace ImprovedModUploadPanel
{
    public class Mod : IUserMod
    {
        public static bool bootstrapped;

        public string Name
        {
            get
            {
                if (!bootstrapped)
                {
                    ImprovedModUploadPanel.Initialize();
                    bootstrapped = true;
                }
                return "Improved Mod Upload Panel";
            }
        }

        public string Description => "Improved Mod Upload Panel";
    }
}
