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

namespace SkyBuy.View;

public sealed partial class SchedulePlanification : Page
{
    public SchedulePlanificationViewModel ViewModel { get; private set; }
    public SchedulePlanification()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<SchedulePlanificationViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e) /// when navigating to this frame
    {
        base.OnNavigatedTo(e); /// setup

        if (e.Parameter is string Airline_Division)
        {
            ViewModel.Set_Airline_Division_Iata(Airline_Division);
            _ = ViewModel.InitAircraftICAOs(Airline_Division);
        }

        ViewModel.InitSeasons();
        
    }   

}
