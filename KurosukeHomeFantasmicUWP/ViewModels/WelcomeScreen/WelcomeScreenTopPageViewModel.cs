﻿using CommonUtils;
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
using KurosukeHomeFantasmicUWP.Utils;
using KurosukeHomeFantasmicUWP.Utils.Auth;
using KurosukeHomeFantasmicUWP.Utils.RequestHelpers;

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
                    AppGlobalVariables.ProjectFile = file;
                    AppGlobalVariables.ProjectFolder = await file.GetParentAsync();
                    AppGlobalVariables.AssetsFolder = await AppGlobalVariables.ProjectFolder.CreateFolderAsync("Assets", Windows.Storage.CreationCollisionOption.OpenIfExists);

                    LoadingMessage = "Loading User Accounts...";
                    AppGlobalVariables.DeviceUsers = new ObservableCollection<AuthCommon.Models.IUser>(await AccountManager.GetAuthorizedUserList());

                    LoadingMessage = "Loading project settings...";
                    using (var projectJsonStream = await file.OpenReadAsync())
                    {
                        AppGlobalVariables.CurrentProject = await JsonSerializer.DeserializeAsync<FantasmicProject>(projectJsonStream.AsStream());
                    }

                    LoadingMessage = "Loading Hue Assets...";
                    try
                    {
                        var availableLights = (from light in await Utils.RequestHelpers.HueRequestHelper.GetHueLights()
                                               select light.HueEntertainmentLight).ToList();
                        AppGlobalVariables.HueAssetDB = new Utils.DBHelpers.HueAssetHelper();
                        var hueData = await AppGlobalVariables.HueAssetDB.GetHueAssets();
                        OnMemoryCache.HueActions = new ObservableCollection<KurosukeHueClient.Models.HueObjects.HueAction>(
                            from action in hueData.HueActions
                            select action.ToHueAction(availableLights)
                            );
                        OnMemoryCache.HueEffects = new ObservableCollection<KurosukeHueClient.Models.HueObjects.HueEffect>(
                            from effect in hueData.HueEffects
                            select effect.ToHueEffect(availableLights)
                            );
                    }
                    catch (InvalidOperationException)
                    {
                        // Ignore if the Hue Bridge/Entertainment Groups is not selected
                        OnMemoryCache.HueActions = new ObservableCollection<KurosukeHueClient.Models.HueObjects.HueAction>();
                        OnMemoryCache.HueEffects = new ObservableCollection<KurosukeHueClient.Models.HueObjects.HueEffect>();
                    }

                    LoadingMessage = "Updating Remote Devices list...";
                    try
                    {
                        await BonjourHelper.UpdateBonjourDevices();
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.WriteErrorLog(ex, "Failed to find local network devices.");
                    }

                    LoadingMessage = "Loading Remote Video Assets...";
                    AppGlobalVariables.RemoteVideoAssetDB = new Utils.DBHelpers.RemoteVideoAssetHelper();
                    OnMemoryCache.RemoteVideoAssets = new ObservableCollection<RemoteVideoAsset>(await AppGlobalVariables.RemoteVideoAssetDB.GetRemoteVideoAssets());

                    LoadingMessage = "Loading Video Asset files...";
                    AppGlobalVariables.VideoAssetDB = new Utils.DBHelpers.VideoAssetsHelper();
                    var videoAssets = new ObservableCollection<VideoAsset>();
                    var videos = await AppGlobalVariables.VideoAssetDB.GetVideoAssets();
                    foreach (var videoAssetEntity in videos) { videoAssets.Add(new VideoAsset(videoAssetEntity)); }
                    OnMemoryCache.VideoAssetCache = videoAssets;

                    LoadingMessage = "Loading Scene data...";
                    AppGlobalVariables.SceneAssetDB = new Utils.DBHelpers.SceneAssetHelper();
                    OnMemoryCache.Scenes = new ObservableCollection<ShowScene>(await AppGlobalVariables.SceneAssetDB.GetSceneAssets());
                }
            }
            catch (Exception ex)
            {
                await DebugHelper.ShowErrorDialog(ex, "プロジェクトを開くことができません。(" + LoadingMessage + ")");
            }

            IsLoading = false;
            IsButtonEnabled = true;

            return AppGlobalVariables.CurrentProject != null;
        }
    }
}
