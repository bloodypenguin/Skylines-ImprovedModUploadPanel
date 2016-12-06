using System;
using System.Reflection;
using ColossalFramework.Packaging;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using UnityEngine;
using Object = System.Object;

namespace ImprovedModUploadPanel.Detours
{
    public class PackageEntryDetour : PackageEntry
    {
        private static RedirectCallsState _state;

        public static void Initialize()
        {
            _state = RedirectionHelper.RedirectCalls
            (
                typeof(PackageEntry).GetMethod("ShareRoutine",
                    BindingFlags.Instance | BindingFlags.NonPublic),
                typeof(PackageEntryDetour).GetMethod("ShareRoutine",
                    BindingFlags.Instance | BindingFlags.NonPublic)
            );
        }

        public static void Revert()
        {
            RedirectionHelper.RevertRedirect(
                typeof(PackageEntry).GetMethod("ShareRoutine",
                    BindingFlags.Instance | BindingFlags.NonPublic),
                _state
            );
        }

        //redirect
        private void ShareRoutine()
        {
            if (this.m_PluginInfo != null)
            {
                WorkshopModUploadPanel workshopModUploadPanel = UIView.library.ShowModal<WorkshopModUploadPanel>("WorkshopModUploadPanel");
                if ((Object)workshopModUploadPanel == (Object)null)
                    return;

                try
                {
                    var instances = pluginInfo.GetInstances<IUserMod>();
                    if (instances.Length == 1)
                    {
                        WorkshopModUploadPanelDetour.Title = instances[0].Name;
                        WorkshopModUploadPanelDetour.Description = $"[h1]{instances[0].Description}[/h1]";
                    }
                    else
                    {
                        DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, "Multiple or no IUserMod implemented for the mod. Only one IUserMod is accepted per mod. " + pluginInfo.ToString());
                    }
                }
                catch (UnityException ex)
                {
                    Debug.LogException((Exception)ex);
                    UIView.ForwardException((Exception)new ModException("A Mod caused an error", (Exception)ex));
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    UIView.ForwardException((Exception)new ModException("A Mod caused an error", ex));
                }


                workshopModUploadPanel.SetAsset(this.m_PluginInfo.modPath, this.m_PublishedFileId);
            }
            else
            {
                if (!(this.asset != (Package.Asset)null) || !(this.asset.type == UserAssetType.MapMetaData) && !(this.asset.type == UserAssetType.ScenarioMetaData) && (!(this.asset.type == UserAssetType.SaveGameMetaData) && !(this.asset.type == UserAssetType.CustomAssetMetaData)) && (!(this.asset.type == UserAssetType.ColorCorrection) && !(this.asset.type == UserAssetType.DistrictStyleMetaData) && (!(this.asset.type == UserAssetType.MapThemeMetaData) && !(this.asset.type == UserAssetType.ScenarioMetaData))))
                    return;
                WorkshopAssetUploadPanel assetUploadPanel = UIView.library.ShowModal<WorkshopAssetUploadPanel>("WorkshopAssetUploadPanel");
                if ((Object)assetUploadPanel == (Object)null)
                    return;
                assetUploadPanel.SetAsset(this.asset, this.m_PublishedFileId);
            }
        }
    }
}