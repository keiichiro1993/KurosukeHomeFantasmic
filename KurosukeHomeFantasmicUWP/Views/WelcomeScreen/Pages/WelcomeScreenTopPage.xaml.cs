using KurosukeHomeFantasmicUWP.ViewModels.WelcomeScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace KurosukeHomeFantasmicUWP.Views.WelcomeScreen.Pages
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class WelcomeScreenTopPage : Page
    {
        public WelcomeScreenTopPageViewModel ViewModel { get; set; } = new WelcomeScreenTopPageViewModel();
        public WelcomeScreenTopPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.Init();
        }

        private void CreateNewProjectButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.IsButtonEnabled = false;
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardConnectedAnimation", (Button)sender);
            Frame.Navigate(typeof(CreateNewProjectPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
            ViewModel.IsButtonEnabled = true;
        }

        private async void OpenProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (await ViewModel.OpenProject())
            {
                Utils.UIHelpers.WindowLauncher.MoveToWorkspaceWindow();
            }
        }
    }
}
