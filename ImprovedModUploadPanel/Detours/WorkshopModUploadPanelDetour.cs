using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.PlatformServices;
using ColossalFramework.UI;
using ImprovedModUploadPanel.Redirection;
using UnityEngine;

namespace ImprovedModUploadPanel.Detours
{
    [TargetType(typeof(WorkshopModUploadPanel))]
    public class WorkshopModUploadPanelDetour : WorkshopModUploadPanel
    {
        private static readonly string[] PreviewImageFilenames = { "PreviewImage.png", "preview.png", "Preview.png" };
        public static string Title = "<YOUR MOD NAME>";
        public static string Description = "<YOUR MOD DESCRIPTION>";

        [RedirectMethod]
        private void UpdateItem()
        {
            try
            {
                string text1 = this.m_Title.text;
                string text2 = this.m_Desc.text;
                string text3 = this.m_ChangeNote.text;
                string[] files = Directory.GetFiles(this.m_ContentPath, "*.*", SearchOption.AllDirectories);
                if (true)
                {
                    try
                    {
                        DirectoryUtils.DeleteDirectory(Path.Combine(this.m_ContentPath, "Source"));
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        ex.HelpLink = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        CODebugBase<LogChannel>.Error(LogChannel.Core, "Failed to delete source for " + this.m_ContentPath + "\n" + ex.ToString());
                    }
                }
                WorkshopHelper.VerifyAndFinalizeFiles(files, this.m_PublishedFileId.ToString());
                //begin mod
                string version = BuildConfig.applicationVersion;
                this.m_CurrentHandle = PlatformService.workshop.UpdateItem(this.m_PublishedFileId, text1, text2, text3, this.m_PreviewPath, this.m_ContentPath, new string[2]
                {
                    !IsCameraScript() ? "Mod" : "Cinematic Cameras",
                    $"{version}-compatible"
                });
                //end mod
            }
            catch (Exception ex)
            {
                CODebugBase<LogChannel>.Error(LogChannel.Core, ex.GetType().ToString() + " " + ex.Message);
                UIView.ForwardException((Exception)new StagingException("Workshop Staging Failed", ex));
            }
        }


        [RedirectMethod]
        private void SetAssetInternal(string folder)
        {
            this.m_StagingPath = (string)null;
            this.m_PreviewPath = (string)null;
            this.m_ContentPath = (string)null;
            this.m_CurrentHandle = UGCHandle.invalid;
            this.m_ShareButton.isEnabled = false;
            this.m_TargetFolder = folder;
            this.m_UpdateText.isVisible = false;
            this.m_UpdateBar.isVisible = false;
            this.m_ShareButton.isEnabled = true;
            //begin mod
            bool isUpdate = m_ShareButton.localeID == "WORKSHOP_UPDATE";
            if (!isUpdate)
            {
                this.m_Title.text = Title;
                this.m_Desc.text = Description;
                this.m_Title.readOnly = false;
                this.m_Desc.readOnly = false;
                this.m_ChangeNote.text = "Initial release";
            }
            else
            {
                this.m_ChangeNote.text = string.Empty;
            }
            //end mod
            this.PrepareStagingArea((Texture)null);
        }

        [RedirectMethod]
        private void PrepareStagingArea(Texture previewTexture)
        {
            this.m_StagingPath = Path.Combine(Path.Combine(DataLocation.localApplicationData, "WorkshopStagingArea"), Guid.NewGuid().ToString());
            Directory.CreateDirectory(this.m_StagingPath);
            //begin mod
            this.m_PreviewPath = Path.Combine(this.m_StagingPath, "PreviewImage.png");
            foreach (var previewImageFilename in PreviewImageFilenames)
            {
                var sourceFileName = Path.Combine(this.m_TargetFolder, previewImageFilename);
                if (File.Exists(sourceFileName))
                {
                    File.Copy(sourceFileName, m_PreviewPath);
                    break;
                }
            }

            if (!File.Exists(m_PreviewPath))
            {
                var texture2D = UnityEngine.Object.Instantiate((Texture)m_DefaultModPreviewTexture) as Texture2D;
                if (texture2D != null)
                    File.WriteAllBytes(m_PreviewPath, texture2D.EncodeToPNG());
            }
            //end mod
            ReloadPreviewImage();
            this.m_ContentPath = Path.Combine(this.m_StagingPath, "Content" + (object)Path.DirectorySeparatorChar);
            Directory.CreateDirectory(this.m_ContentPath);
            WorkshopHelper.DirectoryCopy(this.m_TargetFolder, this.m_ContentPath, true);
            this.StartWatchingPath();
        }

        [RedirectReverse]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ReloadPreviewImage()
        {
            UnityEngine.Debug.Log("ReloadPreviewImage");
        }

        [RedirectReverse]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void StartWatchingPath()
        {
            UnityEngine.Debug.Log("StartWatchingPath");
        }
        
        [RedirectReverse]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool IsCameraScript()
        {
            UnityEngine.Debug.Log("IsCameraScript");
            return false;
        }

        private PublishedFileId m_PublishedFileId => (PublishedFileId)this.GetType().GetField("m_PublishedFileId", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
        private UITextField m_Title => (UITextField)this.GetType().GetField("m_Title", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
        private UITextField m_Desc => (UITextField)this.GetType().GetField("m_Desc", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
        private UITextField m_ChangeNote => (UITextField)this.GetType().GetField("m_ChangeNote", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
        private UIButton m_ShareButton => (UIButton)this.GetType().GetField("m_ShareButton", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
        private UILabel m_UpdateText => (UILabel)this.GetType().GetField("m_UpdateText", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
        private UIProgressBar m_UpdateBar => (UIProgressBar)this.GetType().GetField("m_UpdateBar", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
        private string m_TargetFolder
        {
            get
            {
                return
                    (string)
                    this.GetType()
                        .GetField("m_TargetFolder", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(this);
            }
            set
            {
                this.GetType()
                      .GetField("m_TargetFolder", BindingFlags.NonPublic | BindingFlags.Instance)
                      .SetValue(this, value);
            }
        }
        private string m_ContentPath
        {
            get
            {
                return
                    (string)
                    this.GetType()
                        .GetField("m_ContentPath", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(this);
            }
            set
            {
                this.GetType()
                      .GetField("m_ContentPath", BindingFlags.NonPublic | BindingFlags.Instance)
                      .SetValue(this, value);
            }
        }
        private string m_PreviewPath
        {
            get
            {
                return
                    (string)
                    this.GetType()
                        .GetField("m_PreviewPath", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(this);
            }
            set
            {
                this.GetType()
                      .GetField("m_PreviewPath", BindingFlags.NonPublic | BindingFlags.Instance)
                      .SetValue(this, value);
            }
        }
        private string m_StagingPath
        {
            get
            {
                return (string)this.GetType()
                    .GetField("m_StagingPath", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(this);
            }
            set
            {
                this.GetType()
                      .GetField("m_StagingPath", BindingFlags.NonPublic | BindingFlags.Instance)
                      .SetValue(this, value);
            }
        }
        private UGCHandle m_CurrentHandle
        {
            set
            {
                this.GetType()
                      .GetField("m_CurrentHandle", BindingFlags.NonPublic | BindingFlags.Instance)
                      .SetValue(this, value);
            }
        }
    }


}