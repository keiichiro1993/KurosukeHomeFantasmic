using CommonUtils;
using KurosukeHomeFantasmicUWP.Models;
using KurosukeHomeFantasmicUWP.Models.SQL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

                    LoadingMessage = "Loading User Accounts...";
                    Utils.AppGlobalVariables.DeviceUsers = new ObservableCollection<AuthCommon.Models.IUser>(await Utils.Auth.AccountManager.GetAuthorizedUserList());

                    LoadingMessage = "Loading project settings...";
                    using (var projectJsonStream = await file.OpenReadAsync())
                    {
                        Utils.AppGlobalVariables.CurrentProject = await JsonSerializer.DeserializeAsync<FantasmicProject>(projectJsonStream.AsStream());
                    }

                    LoadingMessage = "Loading Hue Assets...";
                    try
                    {
                        var availableLights = (from light in await Utils.RequestHelpers.HueRequestHelper.GetHueLights()
                                               select light.HueEntertainmentLight).ToList();
                        Utils.AppGlobalVariables.HueAssetDB = new Utils.DBHelpers.HueAssetHelper();
                        var hueData = await Utils.AppGlobalVariables.HueAssetDB.GetHueAssets();
                        Utils.OnMemoryCache.HueActions = new ObservableCollection<KurosukeHueClient.Models.HueObjects.HueAction>(
                            from action in hueData.HueActions
                            select action.ToHueAction(availableLights)
                            );
                        Utils.OnMemoryCache.HueEffects = new ObservableCollection<KurosukeHueClient.Models.HueObjects.HueEffect>(
                            from effect in hueData.HueEffects
                            select effect.ToHueEffect(availableLights, Utils.OnMemoryCache.HueActions.ToList())
                            );
                    }
                    catch (InvalidOperationException)
                    {
                        // Ignore if the Hue Bridge/Entertainment Groups is not selected
                        Utils.OnMemoryCache.HueActions = new ObservableCollection<KurosukeHueClient.Models.HueObjects.HueAction>();
                        Utils.OnMemoryCache.HueEffects = new ObservableCollection<KurosukeHueClient.Models.HueObjects.HueEffect>();
                    }

                    LoadingMessage = "Loading Video Asset files...";
                    Utils.AppGlobalVariables.VideoAssetDB = new Utils.DBHelpers.VideoAssetsHelper();
                    var videoAssets = new ObservableCollection<VideoAsset>();
                    var videos = await Utils.AppGlobalVariables.VideoAssetDB.GetVideoAssets();
                    foreach (var videoAssetEntity in videos) { videoAssets.Add(new VideoAsset(videoAssetEntity)); }
                    Utils.OnMemoryCache.VideoAssetCache = videoAssets;

                    LoadingMessage = "Loading Scene data...";
                    Utils.AppGlobalVariables.SceneAssetDB = new Utils.DBHelpers.SceneAssetHelper();
                    Utils.OnMemoryCache.Scenes = new ObservableCollection<ShowScene>(await Utils.AppGlobalVariables.SceneAssetDB.GetSceneAssets());
                }
            }
            catch (Exception ex)
            {
                await DebugHelper.ShowErrorDialog(ex, "プロジェクトを開くことができません。(" + LoadingMessage + ")");
            }

            IsLoading = false;
            IsButtonEnabled = true;

            return Utils.AppGlobalVariables.CurrentProject != null;
        }
    }
}
