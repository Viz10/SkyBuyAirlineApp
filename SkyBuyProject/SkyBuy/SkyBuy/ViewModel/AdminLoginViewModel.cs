using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.IdentityModel.Tokens;
using Microsoft.WindowsAppSDK.Runtime.Packages;
using SkyBuy.Data.Model.DTOs;
using SkyBuy.Services;
using System.Threading.Tasks;
using System;

namespace SkyBuy.ViewModel
{
    public partial class AdminLoginViewModel : ObservableObject
    {

        [ObservableProperty]
        public partial string Username { get; set; }

        [ObservableProperty]
        public partial string Password { get; set; }

        [ObservableProperty]
        public partial string ErrorMsg { get; set; }

        [ObservableProperty]
        public partial bool LoginBad { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotLoading))]
        public partial bool IsLoading { get; set; }
        public bool IsNotLoading => !IsLoading;

        private readonly AuthenticationService _userService;
        public AdminLoginViewModel(AuthenticationService userService) { _userService = userService; }

        [RelayCommand]
        private async Task LoginFunction()
        {
            
            LoginBad = false;
            IsLoading = true;

            if (Username.IsNullOrEmpty() || Password.IsNullOrEmpty())
            {
                ErrorMsg = $"Login data cannot be empty!";
                LoginBad = true;
                return;
            }

            LoginCredentialsDTO logindataDTO = new LoginCredentialsDTO()
            {
                AccountUniqueCredential = Username,
                Password = Password,
                IsAdmin = true
            };

            try
            {
                ErrorMsg = "";
                var loginResult = await _userService.LoginService(logindataDTO);

                if (!loginResult.Success)
                {
                    ErrorMsg = $"Error: {loginResult.Error}";
                    LoginBad = true;
                }
                else
                {
                    var accountResultDTO = loginResult.AccountData;
                    App.AdminNavigationService.NavigateTo("AdminMainDashboard", accountResultDTO);
                }
            }
            catch (Exception e)
            {
                ErrorMsg += e.Message;
                LoginBad = true;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
