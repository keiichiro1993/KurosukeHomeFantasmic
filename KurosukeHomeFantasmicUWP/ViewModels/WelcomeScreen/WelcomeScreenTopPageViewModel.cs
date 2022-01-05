using KurosukeHomeFantasmicUWP.Models.SQL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.ViewModels.WelcomeScreen
{
    public class WelcomeScreenTopPageViewModel : ViewModelBase
    {

        private List<RecentProjectEntity> _RecentProjects;
        public List<RecentProjectEntity> RecentProjects
        {
            get { return _RecentProjects; }
            set
            {
                _RecentProjects = value;
                RaisePropertyChanged();
            }
        }

        private bool _IsButtonEnabled = true;
        public bool IsButtonEnabled
        {
            get { return _IsButtonEnabled; }
            set
            {
                _IsButtonEnabled = value;
                RaisePropertyChanged();
            }
        }

        public async Task Init()
        {
            IsLoading = true;
            try
            {
                using (var dbContext = new RecentProjectContext())
                {
                    RecentProjects = await dbContext.RecentProjects.ToListAsync();
                }
            }
            catch (Exception)
            {
                //TODO
            }
            IsLoading = false;
        }

        public async Task<bool> OpenProject()
        {
            IsButtonEnabled = false;
            IsLoading = true;

            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker
                {
                    ViewMode = Windows.Storage.Pickers.PickerViewMode.List,
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
                };
                picker.FileTypeFilter.Add(".fantproj");

                Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    Utils.AppGlobalVariables.ProjectFile = file;
                    Utils.AppGlobalVariables.ProjectFolder = await file.GetParentAsync();

                    Utils.AppGlobalVariables.AssetsFolder = await Utils.AppGlobalVariables.ProjectFolder.CreateFolderAsync("Assets", Windows.Storage.CreationCollisionOption.OpenIfExists);
                    Utils.AppGlobalVariables.VideoAssetDB = new Utils.DBHelpers.VideoAssetsHelper();
                    using (var projectJsonStream = await file.OpenReadAsync())
                    {
                        Utils.AppGlobalVariables.CurrentProject = await JsonSerializer.DeserializeAsync<Models.FantasmicProject>(projectJsonStream.AsStream());
                    }
                }
            }
            catch (Exception ex)
            {
                await Utils.DebugHelper.ShowErrorDialog(ex, "プロジェクトを開くことができません。");
            }

            IsLoading = false;
            IsButtonEnabled = true;

            return Utils.AppGlobalVariables.CurrentProject != null;
        }
    }
}
