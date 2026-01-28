using Microsoft.UI.Xaml.Controls;
using SkyBuy.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkyBuy.View.Navigations
{
    public class CustomerNavigation : NavigationBase
    {
        public CustomerNavigation(Frame frame) : base(frame) { }
            
        protected override void AddPages() /// xaml pages
        {
            Pages.Add("LoginPage", typeof(LoginPage));
            Pages.Add("RegisterPage", typeof(RegisterPage));
            Pages.Add("CustomerMainDashboard", typeof(CustomerMainDashboard));
            Pages.Add("FlightSelection", typeof(FlightSelection));
        }
    }
}
