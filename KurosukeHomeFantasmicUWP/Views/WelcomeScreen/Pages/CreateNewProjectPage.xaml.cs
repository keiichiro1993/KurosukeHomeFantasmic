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
    public sealed partial class CreateNewProjectPage : Page
    {
        public CreateNewProjectPageViewModel ViewModel = new CreateNewProjectPageViewModel();
        public CreateNewProjectPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("ForwardConnectedAnimation");
            if (anim != null)
            {
                anim.Configuration = new BasicConnectedAnimationConfiguration();
                anim.TryStart(createProjectTitle);
            }

            this.Loaded += CreateNewProjectPage_Loaded;
        }

        private async void CreateNewProjectPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.Init();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (await ViewModel.CreateButton_Click())
            {
                Utils.UIHelpers.WindowLauncher.MoveToWorkspaceWindow();
            }
        }
    }
}
