
using Microsoft.UI.Xaml.Controls;
using SkyBuy.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace SkyBuy.View
{
    public sealed partial class LoginPage : Page
    {
        public LoginViewModel LoginViewModel { get; private set; } 

        public LoginPage()
        {
            InitializeComponent();
            LoginViewModel = App.Services.GetRequiredService<LoginViewModel>();
        }
    }
}
