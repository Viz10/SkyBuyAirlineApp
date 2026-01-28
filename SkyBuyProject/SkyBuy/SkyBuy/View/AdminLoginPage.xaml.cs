using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using SkyBuy.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace SkyBuy.View
{
    public sealed partial class AdminLoginPage : Page
    {
        public AdminLoginViewModel ViewModel { get; private set; }
        public AdminLoginPage()
        {
            InitializeComponent();
            ViewModel = App.Services.GetRequiredService<AdminLoginViewModel>();
        }
    }
}
