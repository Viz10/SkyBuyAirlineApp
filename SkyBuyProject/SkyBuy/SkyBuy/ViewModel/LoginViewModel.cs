using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using SkyBuy.Data.Model.DTOs;
using Microsoft.IdentityModel.Tokens;
using SkyBuy.Services;
using System.Data.Common;
using System;

namespace SkyBuy.ViewModel
{
    public partial class LoginViewModel : ObservableObject
    {

        private readonly AuthenticationService _userService;
        public LoginViewModel(AuthenticationService userService) { _userService = userService; }


        [ObservableProperty]
        public partial string Email { get; set; }

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
        

        
        
        [RelayCommand]
        private async Task LoginFunction()
        {
            LoginBad = false;
            IsLoading = true;

            if (Email.IsNullOrEmpty() || Password.IsNullOrEmpty())
            {
                ErrorMsg = "Login data cannot be empty!";
                LoginBad= true;
                IsLoading = false;
                return;
            }

            LoginCredentialsDTO logindataDTO = new LoginCredentialsDTO()
            {
                AccountUniqueCredential = Email,
                Password = Password,
                IsAdmin = false
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
                    App.CustomerNavigationService.NavigateTo("CustomerMainDashboard", accountResultDTO);
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
       
        [RelayCommand]
        private static void Register()
        {
            App.CustomerNavigationService.NavigateTo("RegisterPage");
        }

    }
}