using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SkyBuy.Data.Model.DTOs;
using SkyBuy.ViewModel;
using System.Linq;

namespace SkyBuy.View
{
    public sealed partial class CustomerMainDashboard : Page
    {
        public CustomerMainDashBoardViewModel viewModel { get; private set; }

        public CustomerMainDashboard()
        {
            InitializeComponent();
            viewModel = App.Services.GetRequiredService<CustomerMainDashBoardViewModel>();
        }

        /// DATE FUNCTIONS
        private void FirstCalendar_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            viewModel.FirstDate = sender.SelectedDates.FirstOrDefault();
        }
        private void SecondCalendar_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            viewModel.SecondDate = sender.SelectedDates.FirstOrDefault();
        }
        private void CalendarView_DayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            if (viewModel.BlockedDates.Contains(args.Item.Date))
            {
                args.Item.IsBlackout = true;
            }
        }


        /// SUGGESTION BOX FUNCTIONS - DEPARTURE 
        private void AutoSuggestBox1_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                viewModel.TextChangedDepartureCommand.Execute(sender.Text);
            }
        }
        private void AutoSuggestBox1_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is string selected && selected != "Nothing found")
            {
                viewModel.SuggestionChosenCommand.Execute((selected, 1));
            }
        }

        /// SUGGESTION BOX FUNCTIONS - ARRIVAL 
        private void AutoSuggestBox2_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                viewModel.TextChangedArrivalCommand.Execute(sender.Text);
            }
        }
        private void AutoSuggestBox2_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is string selected && selected != "Nothing found")
            {
                viewModel.SuggestionChosenCommand.Execute((selected, 2));
            }
        }


        private async void BookingsFlyout_Opened(object sender, object e)
        {
            await viewModel.LoadUserBookingsAsync();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e) /// when navigating to this frame
        {
            base.OnNavigatedTo(e); /// setup
            if (e.Parameter is AccountWelcomeDataDTO data) // if its that type, down-cast it from object
            {
                viewModel.receiveCustomerData(data); /// DI to viewModel
            }
        }
    }
}