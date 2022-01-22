using KurosukeHomeFantasmicUWP.Controls.ContentDialogs;
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

namespace KurosukeHomeFantasmicUWP.Controls
{
    public sealed partial class ProjectMenuTitleBar : UserControl
    {
        public ProjectMenuTitleBar()
        {
            this.InitializeComponent();
            this.Loaded += ProjectMenuTitleBar_Loaded;
        }

        private void ProjectMenuTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            var menuWidth = menuBarPanel.ActualWidth;
            titleTextBlock.Margin = new Thickness(-(menuWidth + 27), 0, 0, 0);

            Window.Current.SetTitleBar(titleGrid);
        }

        public Frame MainFrame
        {
            get => (Frame)GetValue(MainFrameProperty);
            set => SetValue(MainFrameProperty, value);
        }

        public static readonly DependencyProperty MainFrameProperty =
          DependencyProperty.Register(nameof(MainFrame), typeof(Frame), typeof(ProjectMenuTitleBar), new PropertyMetadata(null, null));


        #region Handling Menu Button clicks

        private async void SaveMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveDialog();
            await saveDialog.ShowAsync();
        }

        private void SettingsMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame != null)
            {
                MainFrame.Navigate(typeof(Views.Settings.SettingsMainPage));
            }
        }

        #endregion
    }
}
