using KurosukeHomeFantasmicUWP.Controls.ContentDialogs;
using KurosukeHomeFantasmicUWP.ViewModels.Auth;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace KurosukeHomeFantasmicUWP.Views.Auth
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class AuthDialogMainPage : Page
    {
        public AuthDialogMainPageViewModel ViewModel { get; set; } = new AuthDialogMainPageViewModel();
        private AuthDialog dialogHost;

        public AuthDialogMainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            dialogHost = e.Parameter as AuthDialog;
            dialogHost.Closing += ContentDialog_Closing;
        }

        void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            /*if (!ViewModel.IsLoading)
            {
                args.Cancel = true;
            }*/
        }

        private void PhillipsHue_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.IsButtonAvailable = false;
            Frame.Navigate(typeof(AuthDialogHuePage), dialogHost, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }
    }
}
