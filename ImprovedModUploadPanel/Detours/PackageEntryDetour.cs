using System;
using System.Reflection;
using ColossalFramework.Packaging;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using ImprovedModUploadPanel.Redirection;
using UnityEngine;
using Object = System.Object;

namespace ImprovedModUploadPanel.Detours
{
    [TargetType(typeof(PackageEntry))]
    public class PackageEntryDetour : PackageEntry
    {
        [RedirectMethod]
        private void ShareRoutine()
        {
            if (this.pluginInfo != null)
            {
                WorkshopModUploadPanel workshopModUploadPanel = UIView.library.ShowModal<WorkshopModUploadPanel>("WorkshopModUploadPanel");
                if ((Object)workshopModUploadPanel == (Object)null)
                    return;
                //begin mod
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
                //end mod
                workshopModUploadPanel.SetAsset(this.pluginInfo.modPath, this.publishedFileId);
            }
            else
            {
                if (!(this.asset != (Package.Asset)null) || !(this.asset.type == UserAssetType.MapMetaData) && !(this.asset.type == UserAssetType.ScenarioMetaData) && (!(this.asset.type == UserAssetType.SaveGameMetaData) && !(this.asset.type == UserAssetType.CustomAssetMetaData)) && (!(this.asset.type == UserAssetType.ColorCorrection) && !(this.asset.type == UserAssetType.DistrictStyleMetaData) && (!(this.asset.type == UserAssetType.MapThemeMetaData) && !(this.asset.type == UserAssetType.ScenarioMetaData))))
                    return;
                WorkshopAssetUploadPanel assetUploadPanel = UIView.library.ShowModal<WorkshopAssetUploadPanel>("WorkshopAssetUploadPanel");
                if ((Object)assetUploadPanel == (Object)null)
                    return;
                assetUploadPanel.SetAsset(this.asset, this.publishedFileId);
            }
        }
    }
}