using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.IdentityModel.Tokens;
using SkyBuy.Data.Model.DTOs;
using SkyBuy.Services;
using SkyBuy.Common.Helpers;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SkyBuy.ViewModel
{
    public partial class AdminMainDashBoardViewModel : ObservableObject
    {

        private AccountWelcomeDataDTO _accountWelcomeDataDTO;
        private AirLabsClient _AirLabsClient;
        private AirportsClient _AirportsClient;



        [ObservableProperty]
        public partial string WelcomeString { get; set; }
        
        [ObservableProperty]
        public partial string ErrorString { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotLoading))]
        public partial bool IsLoading { get; set; }

        public bool IsNotLoading => !IsLoading;
        
        [ObservableProperty]
        public partial bool HasErrors { get; set; }
        
        [ObservableProperty]
        public partial string Airline_Division_Iata { get; set; } /// selected item

        [ObservableProperty]
        public partial ObservableCollection<string> Airline_Divisions { get; set; } = ["W6", "W4"];
     




        public AdminMainDashBoardViewModel(AirLabsClient airLabsClient, AirportsClient airportsClient)
        {
            _AirLabsClient = airLabsClient;
            _AirportsClient = airportsClient;
        }



        [RelayCommand]
        public async Task AddAirports()
        {
            IsLoading = true;
            await _AirportsClient.LoadAirportsFromGitHub();
            IsLoading = false;
        }

        [RelayCommand]
        public async Task AddAircrafts()
        {
            IsLoading = true;
            HasErrors = false;
            ErrorString = "";

            if (string.IsNullOrEmpty(Airline_Division_Iata))
            {
                HasErrors = true;
                ErrorString = "Please select Company branch!";
                IsLoading = false;
                return;
            }

            await _AirLabsClient.LoadAircraftsAsync(Airline_Division_Iata);
            IsLoading = false;
        }

        [RelayCommand]
        public void AddSchedule()
        {
            HasErrors = false;
            ErrorString = "";

            if (string.IsNullOrEmpty(Airline_Division_Iata))
            {
                HasErrors = true;
                ErrorString = "Please select Company branch!";
                return;
            }
            App.AdminNavigationService.NavigateTo("SchedulePlanification",Airline_Division_Iata);
        }




        [RelayCommand]
        private void GoBackLoginPage()
        {
            App.AdminNavigationService.GoBack();
        }
        internal void receiveAdminData(AccountWelcomeDataDTO data)
        {
            _accountWelcomeDataDTO = data;
            WelcomeString = _accountWelcomeDataDTO.Username;
        }
    }
}
