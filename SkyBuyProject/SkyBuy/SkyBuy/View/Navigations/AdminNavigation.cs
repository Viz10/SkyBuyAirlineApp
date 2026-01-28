using Microsoft.UI.Xaml.Controls;
using SkyBuy.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkyBuy.View.Navigations
{
    public class AdminNavigation : NavigationBase
    {
        public AdminNavigation(Frame frame) : base(frame) { }

        protected override void AddPages()
        {
            Pages.Add("AdminLoginPage", typeof(AdminLoginPage));
            Pages.Add("AdminMainDashboard", typeof(AdminMainDashboard));
            Pages.Add("SchedulePlanification", typeof(SchedulePlanification));
        }
    }
}
