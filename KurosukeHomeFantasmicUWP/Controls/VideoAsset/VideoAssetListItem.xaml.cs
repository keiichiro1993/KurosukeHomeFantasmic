using CommonUtils;
using KurosukeHomeFantasmicUWP.Models.JSON;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// ユーザー コントロールの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace KurosukeHomeFantasmicUWP.Controls.VideoAsset
{
    public sealed partial class VideoAssetListItem : UserControl
    {
        public VideoAssetListItemViewModel ViewModel { get; set; } = new VideoAssetListItemViewModel();
        public VideoAssetListItem()
        {
            this.InitializeComponent();
        }

        public Models.VideoAsset VideoAsset
        {
            get => (Models.VideoAsset)GetValue(VideoAssetProperty);
            set => SetValue(VideoAssetProperty, value);
        }

        public static readonly DependencyProperty VideoAssetProperty =
          DependencyProperty.Register(nameof(VideoAsset), typeof(Models.VideoAsset), typeof(VideoAssetListItem),
              new PropertyMetadata(null, VideoAssetEntityChanged));

        private static void VideoAssetEntityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (VideoAssetListItem)d;
            if (instance.IsLoaded)
            {
                instance.ViewModel.Init(e.NewValue as Models.VideoAsset);
            }
        }

        public new bool CanDrag
        {
            get => (bool)GetValue(CanDragProperty);
            set => SetValue(CanDragProperty, value);
        }

        public static readonly new DependencyProperty CanDragProperty =
          DependencyProperty.Register(nameof(CanDrag), typeof(bool), typeof(VideoAssetListItem), null);

        private void Grid_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            args.Data.Properties.Add("VideoAsset", VideoAsset);
        }
    }

    public class VideoAssetListItemViewModel : ViewModels.ViewModelBase
    {
        private Models.VideoAsset _VideoAsset;
        public Models.VideoAsset VideoAsset
        {
            get { return _VideoAsset; }
            set
            {
                _VideoAsset = value;
                RaisePropertyChanged();
            }
        }

        public async void Init(Models.VideoAsset videoAsset)
        {
            IsLoading = true;

            if (videoAsset != null)
            {
                // check video file existence
                try
                {
                    await videoAsset.GetVideoAssetFile();
                }
                catch (Exception ex)
                {
                    await DebugHelper.ShowErrorDialog(ex, "ビデオアセットの読み込みに失敗しました。対象のビデオ " + VideoAsset.VideoAssetEntity.FilePath + " が存在しない可能性があります。");
                }

                this.VideoAsset = videoAsset;
            }

            IsLoading = false;
        }
    }
}
