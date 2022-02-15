using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// コンテンツ ダイアログの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace KurosukeHomeFantasmicUWP.Controls.ContentDialogs
{
    public sealed partial class SaveDialog : ContentDialog
    {
        public SaveDialogViewModel ViewModel { get; set; } = new SaveDialogViewModel();
        public SaveDialog()
        {
            this.InitializeComponent();
            this.Loaded += SaveDialog_Loaded;
        }

        private async void SaveDialog_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //save scenes
                foreach (var item in Utils.OnMemoryCache.Scenes)
                {
                    foreach (var timeline in item.Timelines)
                    {
                        timeline.EncodeTimelineItemToEntity();
                    }
                }           
                await Utils.AppGlobalVariables.SceneAssetDB.SaveSceneAssets(Utils.OnMemoryCache.Scenes.ToList());
                //save project settings
                var projectJson = JsonSerializer.Serialize(Utils.AppGlobalVariables.CurrentProject);
                await FileIO.WriteTextAsync(Utils.AppGlobalVariables.ProjectFile, projectJson);
                
                this.Hide();
            }
            catch (Exception ex)
            {
                ViewModel.ErrorOccured = true;
                ViewModel.ErrorMessage = "Failed to Save : [" + ex.GetType().FullName + "] " + ex.Message;
                ViewModel.IsCloseButtonEnabled = true;
            }
        }
    }

    public class SaveDialogViewModel : ViewModels.ViewModelBase
    {
        private bool _ErrorOccured = false;
        public bool ErrorOccured
        {
            get { return _ErrorOccured; }
            set
            {
                _ErrorOccured = value;
                RaisePropertyChanged();
            }
        }

        private bool _IsCloseButtonEnabled = false;
        public bool IsCloseButtonEnabled
        {
            get { return _IsCloseButtonEnabled; }
            set
            {
                _IsCloseButtonEnabled = value;
                RaisePropertyChanged();
            }
        }

        private string _ErrorMessage;
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set
            {
                _ErrorMessage = value;
                RaisePropertyChanged();
            }
        }
    }
}
