using KurosukeHomeFantasmicUWP.Models;
using KurosukeHomeFantasmicUWP.Models.Timeline;
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
    public sealed partial class AddTimelineDialog : ContentDialog
    {
        public AddTimelineDialogViewModel ViewModel { get; set; } = new AddTimelineDialogViewModel();
        public AddTimelineDialog(ShowScene scene)
        {
            this.InitializeComponent();
            ViewModel.Scene = scene;
            ViewModel.Name = "Timeline" + (scene.Timelines.Count + 1).ToString();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            switch (ViewModel.SelectedType)
            {
                case "Video":
                    var newTimeline = new Models.Timeline.Timeline
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = ViewModel.Name,
                        Description = ViewModel.Description,
                        TimelineType = (Models.Timeline.Timeline.TimelineTypeEnum)Enum.Parse(typeof(Models.Timeline.Timeline.TimelineTypeEnum), ViewModel.SelectedType),
                        TimelineItems = new ObservableCollection<ITimelineItem>(),
                        TargetDisplayId = Guid.NewGuid().ToString()
                    };
                    ViewModel.Scene.Timelines.Add(newTimeline);
                    break;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }

    public class AddTimelineDialogViewModel : ViewModels.ViewModelBase
    {
        public ShowScene Scene { get; set; }

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
                var names = from timeline in Scene.Timelines
                            select timeline.Name;
                IsPrimaryButtonEnabled = !string.IsNullOrEmpty(value) && !names.Contains(value);
            }
        }

        public string Description { get; set; }

        public List<string> TimelineTypes { get; set; } = new List<string> { "Video" };

        public string SelectedType { get; set; } = "Video";
    }
}
