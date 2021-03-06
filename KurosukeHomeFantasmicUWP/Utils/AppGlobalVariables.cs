using AuthCommon.Models;
using KurosukeHomeFantasmicUWP.Models;
using KurosukeHomeFantasmicUWP.Utils.DBHelpers;
using KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace;
using KurosukeHueClient.Models.HueObjects;
using System.Collections.ObjectModel;
using Windows.Storage;

namespace KurosukeHomeFantasmicUWP.Utils
{
    public static class AppGlobalVariables
    {

        public static FantasmicProject CurrentProject;
        public static StorageFolder ProjectFolder;
        public static StorageFile ProjectFile;
        public static StorageFolder AssetsFolder;
        public static VideoAssetsHelper VideoAssetDB;
        public static SceneAssetHelper SceneAssetDB;

        public static ObservableCollection<IUser> DeviceUsers;
    }

    public static class OnMemoryCache
    {
        public static ObservableCollection<VideoAsset> VideoAssetCache; //cache on memory since this holds bitmap images
        public static ObservableCollection<ShowScene> Scenes;
        public static ObservableCollection<HueAction> HueActions = new ObservableCollection<HueAction>();
        public static ObservableCollection<HueEffect> HueEffects = new ObservableCollection<HueEffect>();

        public static ProjectWorkspaceViewModel GlobalViewModel; //Controls playback etc.
    }
}
