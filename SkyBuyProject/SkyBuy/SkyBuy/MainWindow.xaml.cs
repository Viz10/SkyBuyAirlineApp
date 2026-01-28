using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SkyBuy.View.Navigations;
using Windows.System;

namespace SkyBuy
{
    public sealed partial class MainWindow : Window
    {
        CustomerNavigation CustomerNavigationService { get; set; }
        AdminNavigation AdminNavigationService { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            CustomerNavigationService = new CustomerNavigation(customerFrame);
            AdminNavigationService = new AdminNavigation(adminFrame);

            App.CustomerNavigationService = CustomerNavigationService; /// Store them globally
            App.AdminNavigationService = AdminNavigationService;

        }

        private void Begin_Button_Click(object sender, RoutedEventArgs e) /// rediect user
        {
            /// show user mode
            WelcomePanel.Visibility = Visibility.Collapsed;
            adminFrame.Visibility = Visibility.Collapsed;
            customerFrame.Visibility = Visibility.Visible;

            customerFrame.BackStack.Clear(); /// clear navigation history
            App.CustomerNavigationService.NavigateTo("LoginPage");
        }


        /*
        Another way : track key state manually using KeyDown / KeyUp
        Keep a HashSet<VirtualKey> of currently pressed keys
        Add keys on KeyDown
        Remove keys on KeyUp
        Check if all 3 required keys are present
         */
        private void RootGrid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            /// Ctrl+Shift+A
            var ctrlPressed = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control)
                .HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);
            var shiftPressed = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift)
                .HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

            if (ctrlPressed && shiftPressed && e.Key == VirtualKey.A && WelcomePanel.Visibility!=Visibility.Collapsed)
            {
                /// Show admin mode
                WelcomePanel.Visibility = Visibility.Collapsed;
                customerFrame.Visibility = Visibility.Collapsed;
                adminFrame.Visibility = Visibility.Visible;

                adminFrame.BackStack.Clear();
                App.AdminNavigationService.NavigateTo("AdminLoginPage");

                e.Handled = true;
            }
        }
    }
}