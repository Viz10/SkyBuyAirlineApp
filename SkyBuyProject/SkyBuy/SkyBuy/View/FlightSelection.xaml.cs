using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SkyBuy.Data.Model.DTOs;
using SkyBuy.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SkyBuy.View
{
    public sealed partial class FlightSelection : Page
    {
        public FlightSelectionViewModel ViewModel { get; private set; }

        public FlightSelection()
        {
            InitializeComponent();
            ViewModel = App.Services.GetRequiredService<FlightSelectionViewModel>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is FlightSelectDatasDTO data)
            {
                ViewModel.receiveData(data);
            }
        }


    }
}
