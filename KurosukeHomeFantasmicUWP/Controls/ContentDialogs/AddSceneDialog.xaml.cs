using CommonUtils;
using KurosukeHomeFantasmicUWP.Models;
using KurosukeHomeFantasmicUWP.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class AddSceneDialog : ContentDialog
    {
        public AddSceneDialogViewModel ViewModel { get; set; } = new AddSceneDialogViewModel();
        public AddSceneDialog()
        {
            this.InitializeComponent();
            ViewModel.Name = "Scene" + (Utils.OnMemoryCache.Scenes.Count + 1).ToString();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var newScene = new ShowScene();
            newScene.Name = ViewModel.Name;
            newScene.Description = ViewModel.Description;
            newScene.Id = Guid.NewGuid().ToString();
            newScene.Timelines = new ObservableCollection<Models.Timeline.Timeline>();
            Utils.OnMemoryCache.Scenes.Add(newScene);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }

    public class AddSceneDialogViewModel : ViewModelBase
    {
        private bool _IsPrimaryButtonEnabled = true;
        public bool IsPrimaryButtonEnabled
        {
            get { return _IsPrimaryButtonEnabled; }
            set
            {
                _IsPrimaryButtonEnabled = value;
                RaisePropertyChanged();
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                var names = from scene in Utils.OnMemoryCache.Scenes
                            select scene.Name;
                IsPrimaryButtonEnabled = !string.IsNullOrEmpty(value) && !names.Contains(value);
            }
        }

        public string Description { get; set; }
    }
}
