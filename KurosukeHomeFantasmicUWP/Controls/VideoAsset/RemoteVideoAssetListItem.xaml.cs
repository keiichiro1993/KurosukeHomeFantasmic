using CommonUtils;
using KurosukeHomeFantasmicUWP.Models;
using KurosukeHomeFantasmicUWP.Utils.UIHelpers;
using System;
using System.Collections.Generic;
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

// ユーザー コントロールの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace KurosukeHomeFantasmicUWP.Controls.VideoAsset
{
    public sealed partial class RemoteVideoAssetListItem : UserControl
    {
        //TODO: サムネイルのリモートホストへのリクエストを追加したい
        public RemoteVideoAssetListItem()
        {
            this.InitializeComponent();
        }

        public RemoteVideoAsset RemoteVideoAsset
        {
            get => (RemoteVideoAsset)GetValue(RemoteVideoAssetProperty);
            set => SetValue(RemoteVideoAssetProperty, value);
        }

        public static readonly DependencyProperty RemoteVideoAssetProperty =
          DependencyProperty.Register(nameof(RemoteVideoAsset), typeof(RemoteVideoAsset), typeof(RemoteVideoAssetListItem),
              new PropertyMetadata(null, VideoAssetEntityChanged));

        private static void VideoAssetEntityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public delegate void RemoteVideoDeleteButtonClickedEventHandler(object sender, ItemDeleteButtonClickedEventArgs<RemoteVideoAsset> args);
        public event RemoteVideoDeleteButtonClickedEventHandler DeleteButtonClicked;

        public new bool CanDrag
        {
            get => (bool)GetValue(CanDragProperty);
            set => SetValue(CanDragProperty, value);
        }

        public static readonly new DependencyProperty CanDragProperty =
          DependencyProperty.Register(nameof(CanDrag), typeof(bool), typeof(RemoteVideoAssetListItem), null);

        private void Grid_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            args.Data.Properties.Add("RemoteVideoAsset", RemoteVideoAsset);
            args.Data.Properties.Add("TimelineType", "Video");
        }

        private void ContextMenuDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DeleteButtonClicked != null)
            {
                this.DeleteButtonClicked(this, new ItemDeleteButtonClickedEventArgs<RemoteVideoAsset>(RemoteVideoAsset));
            }
        }
    }
}
