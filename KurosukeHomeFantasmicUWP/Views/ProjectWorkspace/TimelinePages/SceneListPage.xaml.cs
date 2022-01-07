using KurosukeHomeFantasmicUWP.Controls.ContentDialogs;
using KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace.TimelinePages;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace KurosukeHomeFantasmicUWP.Views.ProjectWorkspace.TimelinePages
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class SceneListPage : Page
    {
        public SceneListPageViewModel ViewModel { get; set; } = new SceneListPageViewModel();
        public SceneListPage()
        {
            this.InitializeComponent();
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem;
            Frame.Navigate(typeof(SceneDetailsTimelinePage), item, new DrillInNavigationTransitionInfo());
        }

        private async void AddSceneButton_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            var dialog = new AddSceneDialog();
            await dialog.ShowAsync();
            ((Button)sender).IsEnabled = true;
        }
    }
}
