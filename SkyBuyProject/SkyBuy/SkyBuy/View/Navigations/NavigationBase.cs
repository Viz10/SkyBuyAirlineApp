using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkyBuy.View.Navigations
{
    public abstract class NavigationBase
    {
        private readonly Frame _frame;
        public Dictionary<string, object> Pages { get; set; }


        public NavigationBase(Frame frame)
        {
            _frame = frame;
            Pages = new Dictionary<string, object>();
            AddPages();
        }
        

        public bool NavigateTo(string pageName, object any_type = null) /// also send any tipe of data to the next frame
        {
            if (_frame != null && Pages.TryGetValue(pageName, out var pageType)) /// store result into new variable
            {
                _frame.Navigate((Type)pageType, any_type);
                return true;
            }
            return false;
        }
        public bool GoBack()
        {
            if (_frame.CanGoBack)
            {
                _frame.GoBack();
                return true;
            }
            return false;
        }
        protected abstract void AddPages();
    }
}
