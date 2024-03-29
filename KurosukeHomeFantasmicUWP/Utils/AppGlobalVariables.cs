﻿using AuthCommon.Models;
using KurosukeBonjourService;
using KurosukeBonjourService.Models;
using KurosukeHomeFantasmicUWP.Models;
using KurosukeHomeFantasmicUWP.Utils.DBHelpers;
using KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace;
using KurosukeHueClient.Models.HueObjects;
using System.Collections.ObjectModel;
using Windows.Storage;

namespace KurosukeHomeFantasmicUWP.Utils
{
    internal static class AppGlobalVariables
    {
        public static FantasmicProject CurrentProject;
        public static StorageFolder ProjectFolder;
        public static StorageFile ProjectFile;
        public static StorageFolder AssetsFolder;
        public static VideoAssetsHelper VideoAssetDB;
        public static HueAssetHelper HueAssetDB;
        public static SceneAssetHelper SceneAssetDB;
        public static RemoteVideoAssetHelper RemoteVideoAssetDB;

        public static KurosukeHueClient.Utils.HueClient GlobalHueClient;
        public static readonly object HueClientLock = new object();

        public static ObservableCollection<BonjourClient> BonjourClients = new ObservableCollection<BonjourClient>();
        public static ObservableCollection<IUser> DeviceUsers;
    }

    internal static class OnMemoryCache
    {
        public static ObservableCollection<VideoAsset> VideoAssetCache; //cache on memory since this holds bitmap images
        public static ObservableCollection<ShowScene> Scenes;
        public static ObservableCollection<HueAction> HueActions;
        public static ObservableCollection<HueEffect> HueEffects;
        public static ObservableCollection<RemoteVideoAsset> RemoteVideoAssets;
        public static ObservableCollection<QueryResponseItem> BonjourDevices;

        public static ProjectWorkspaceViewModel GlobalViewModel; //Controls playback etc.
    }
}
