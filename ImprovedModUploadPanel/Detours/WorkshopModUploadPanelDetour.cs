using System;
using System.IO;
using System.Reflection;
using ColossalFramework.IO;
using ColossalFramework.PlatformServices;
using ColossalFramework.UI;
using UnityEngine;

namespace ImprovedModUploadPanel.Detours
{
    public class WorkshopModUploadPanelDetour
    {
        private static readonly string[] PreviewImageFilenames = { "PreviewImage.png", "preview.png", "Preview.png" };
        private static WorkshopModUploadPanel _workshopModUploadPanel;


        private static RedirectCallsState _setAssetInternalState;


        private static FieldInfo m_StagingPath;
        private static FieldInfo m_PreviewPath;
        private static FieldInfo m_ContentPath;
        private static FieldInfo m_CurrentHandle;
        private static UIButton m_ShareButton;
        private static FieldInfo m_TargetFolder;
        private static UITextField m_Title;
        private static UITextField m_Desc;
        private static UITextField m_ChangeNote;
        private static FieldInfo m_DefaultModPreviewTexture;

        private static MethodInfo _reloadPreviewImage = typeof(WorkshopModUploadPanel).GetMethod("ReloadPreviewImage",
                BindingFlags.Instance | BindingFlags.NonPublic);
        private static MethodInfo _startWatchingPath = typeof(WorkshopModUploadPanel).GetMethod("StartWatchingPath",
                BindingFlags.Instance | BindingFlags.NonPublic);

        public static string Title = "<YOUR MOD NAME>";
        public static string Description = "<YOUR MOD DESCRIPTION>";

        public static void Deploy()
        {
            _setAssetInternalState = RedirectionHelper.RedirectCalls
            (
                typeof(WorkshopModUploadPanel).GetMethod("SetAssetInternal",
                    BindingFlags.Instance | BindingFlags.NonPublic),
                typeof(ImprovedModUploadPanel).GetMethod("SetAssetInternal",
                    BindingFlags.Instance | BindingFlags.NonPublic)
            );
        }


        public static bool SetupUI()
        {
            var go = GameObject.Find("(Library) WorkshopModUploadPanel");
            if (go == null)
            {
                return false;
            }

            _workshopModUploadPanel = go.GetComponent<WorkshopModUploadPanel>();

            if (_workshopModUploadPanel == null)
            {
                return false;
            }

            m_StagingPath = Util.FindField(_workshopModUploadPanel, "m_StagingPath");
            m_PreviewPath = Util.FindField(_workshopModUploadPanel, "m_PreviewPath");
            m_ContentPath = Util.FindField(_workshopModUploadPanel, "m_ContentPath");
            m_CurrentHandle = Util.FindField(_workshopModUploadPanel, "m_CurrentHandle");
            m_TargetFolder = Util.FindField(_workshopModUploadPanel, "m_TargetFolder");
            m_DefaultModPreviewTexture = Util.FindField(_workshopModUploadPanel, "m_DefaultModPreviewTexture");

            m_Title = _workshopModUploadPanel.Find<UITextField>("Title");
            m_Desc = _workshopModUploadPanel.Find<UITextField>("Desc");
            m_ChangeNote = _workshopModUploadPanel.Find<UITextField>("ChangeNote");
            m_ShareButton = _workshopModUploadPanel.Find<UIButton>("Share");

            return true;
        }

        public static void TearDownUI()
        {
            _workshopModUploadPanel = null;
            m_StagingPath = null;
            m_PreviewPath = null;
            m_ContentPath = null;
            m_CurrentHandle = null;
            m_ShareButton = null;
            m_TargetFolder = null;
            m_ShareButton = null;
            m_Title = null;
            m_Desc = null;
            m_ChangeNote = null;
            m_DefaultModPreviewTexture = null;
        }

        public static void Revert()
        {
            RedirectionHelper.RevertRedirect(typeof(WorkshopModUploadPanel).GetMethod("SetAssetInternal",
                    BindingFlags.Instance | BindingFlags.NonPublic), _setAssetInternalState);
        }


        //redirect
        private void SetAssetInternal(string folder)
        {
            m_StagingPath.SetValue(_workshopModUploadPanel, null);
            m_PreviewPath.SetValue(_workshopModUploadPanel, null);
            m_ContentPath.SetValue(_workshopModUploadPanel, null);
            m_CurrentHandle.SetValue(_workshopModUploadPanel, UGCHandle.invalid);
            m_ShareButton.isEnabled = false;
            m_TargetFolder.SetValue(_workshopModUploadPanel, folder);
            m_ShareButton.isEnabled = true;

            bool isUpdate = m_ShareButton.localeID == "WORKSHOP_UPDATE";

            if (!isUpdate)
            {
                m_Title.text = Title;
                m_Desc.text = Description;
                m_Title.readOnly = false;
                m_Desc.readOnly = false;
                m_ChangeNote.text = "Initial release";
            }
            else
            {
                m_ChangeNote.text = string.Empty;
            }
            PrepareStagingArea(folder);
        }

        //redirect
        private void PrepareStagingArea(string folder)
        {
            string path = Guid.NewGuid().ToString();

            var stagingPath = Path.Combine(Path.Combine(DataLocation.localApplicationData, "WorkshopStagingArea"), path);

            m_StagingPath.SetValue(_workshopModUploadPanel, stagingPath);

            Directory.CreateDirectory(stagingPath);

            var previewPath = Path.Combine(stagingPath, "PreviewImage.png");
            foreach (var previewImageFilename in PreviewImageFilenames)
            {
                var sourceFileName = Path.Combine(folder, previewImageFilename);
                if (File.Exists(sourceFileName))
                {
                    File.Copy(sourceFileName, previewPath);
                    break;
                }
            }

            if (!File.Exists(previewPath))
            {
                var defaultTexture = m_DefaultModPreviewTexture.GetValue(_workshopModUploadPanel);
                var texture2D = UnityEngine.Object.Instantiate((Texture)defaultTexture) as Texture2D;
                if (texture2D != null)
                    File.WriteAllBytes(previewPath, texture2D.EncodeToPNG());
            }

            m_PreviewPath.SetValue(_workshopModUploadPanel, previewPath);
            _reloadPreviewImage.Invoke(_workshopModUploadPanel, null);

            var contentPath = Path.Combine(stagingPath, "Content" + Path.DirectorySeparatorChar);
            m_ContentPath.SetValue(_workshopModUploadPanel, contentPath);

            Directory.CreateDirectory(contentPath);
            WorkshopHelper.DirectoryCopy(folder, contentPath, true);

            _startWatchingPath.Invoke(_workshopModUploadPanel, null);
        }
    }
}