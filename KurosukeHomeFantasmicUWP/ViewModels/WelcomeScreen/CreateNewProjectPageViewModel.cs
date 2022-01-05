﻿using KurosukeHomeFantasmicUWP.Models;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace KurosukeHomeFantasmicUWP.ViewModels.WelcomeScreen
{
    public class CreateNewProjectPageViewModel : ViewModelBase
    {
        private string _ProjectName;
        public string ProjectName
        {
            get { return _ProjectName; }
            set
            {
                _ProjectName = value;
                if (!pathEditedByUser)
                {
                    ProjectDirectoryPath = Path.Combine(documentsLibrary.SaveFolder.Path, "FantasmicProjects", value);
                }
                checkParameters();
            }
        }

        private StorageFolder documentsFolder;
        private StorageLibrary documentsLibrary;
        private bool pathEditedByUser = false;
        private string _ProjectDirectoryPath;
        public string ProjectDirectoryPath
        {
            get { return _ProjectDirectoryPath; }
            set
            {
                _ProjectDirectoryPath = value;
                RaisePropertyChanged();
            }
        }

        private bool _IsCreateButtonEnabled = false;
        public bool IsCreateButtonEnabled
        {
            get { return _IsCreateButtonEnabled; }
            set
            {
                _IsCreateButtonEnabled = value;
                RaisePropertyChanged();
            }
        }

        private void checkParameters()
        {
            IsCreateButtonEnabled = !string.IsNullOrEmpty(ProjectName) && !string.IsNullOrEmpty(ProjectDirectoryPath);
        }

        public async Task Init()
        {
            IsLoading = true;
            documentsFolder = KnownFolders.DocumentsLibrary;
            documentsLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Documents);
            IsLoading = false;
        }


        public async Task<bool> CreateButton_Click()
        {
            IsCreateButtonEnabled = false;
            IsLoading = true;

            var project = new FantasmicProject();
            project.Name = ProjectName;
            project.Version = "1.0.0";
            project.Id = Guid.NewGuid().ToString();

            try
            {
                if (!pathEditedByUser)
                {
                    var projectParentFolder = await documentsFolder.CreateFolderAsync("FantasmicProjects", CreationCollisionOption.OpenIfExists);
                    Utils.AppGlobalVariables.ProjectFolder = await projectParentFolder.CreateFolderAsync(ProjectName, CreationCollisionOption.FailIfExists);
                    Utils.AppGlobalVariables.ProjectFile = await Utils.AppGlobalVariables.ProjectFolder.CreateFileAsync(ProjectName.ToLower() + ".fantproj", CreationCollisionOption.FailIfExists);

                    var projectJson = JsonSerializer.Serialize(project);
                    await FileIO.WriteTextAsync(Utils.AppGlobalVariables.ProjectFile, projectJson);

                    Utils.AppGlobalVariables.AssetsFolder = await Utils.AppGlobalVariables.ProjectFolder.CreateFolderAsync("Assets", Windows.Storage.CreationCollisionOption.OpenIfExists);
                    Utils.AppGlobalVariables.VideoAssetDB = new Utils.DBHelpers.VideoAssetsHelper();
                    Utils.AppGlobalVariables.CurrentProject = project;
                }
            }
            catch (Exception ex)
            {
                await Utils.DebugHelper.ShowErrorDialog(ex, "プロジェクトの作成に失敗しました。同名のプロジェクトが同一パスに存在していないかをご確認ください。");
            }

            IsLoading = false;

            return Utils.AppGlobalVariables.CurrentProject != null;
        }
    }
}
