
using Microsoft.UI.Xaml.Controls;
using SkyBuy.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace SkyBuy.View
{
    public sealed partial class RegisterPage : Page
    {
        public RegisterViewModel RegisterViewModel { get; private set; }
        public RegisterPage()
        {
            InitializeComponent();
            RegisterViewModel = App.Services.GetRequiredService<RegisterViewModel>();
        }
    }
}



