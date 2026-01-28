using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkyBuy.Models;
using SkyBuy.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SkyBuy.ViewModel
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly RegisterDataValidator _validator;
        private readonly AuthenticationService _userService;
        private  List<string> _errorsList { get; set; } 

        public RegisterViewModel(AuthenticationService userService)
        {
            _userService = userService;
            _validator  = new RegisterDataValidator();
            _errorsList = new List<string>();
        }

        [ObservableProperty]
        public partial ObservableCollection<string> ValidationErrors { get; set; } = new ObservableCollection<string>(); /// list of UI displayed validation errors

        [ObservableProperty]
        public partial string Email { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Password { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string ConfirmPassword { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string FirstName { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string LastName { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Phone { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Address { get; set; } = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NotAccepted))]
        public partial bool Accepted { get; set; }
        public bool NotAccepted => !Accepted;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NotLoading))]
        public partial bool Loading { get; set; } = false;
        public bool NotLoading => !Loading;

       
        
        

        
        
        [RelayCommand]
        public async Task TryRegister()
        {
            Loading = true;
            Accepted = false;
            ValidationErrors.Clear();

            try
            {
                var registerDto = new RegisterFormCustomerDTO
                {
                    Email = Email.Trim(),
                    Password = Password.Trim(),
                    ConfirmPassword = ConfirmPassword.Trim(),
                    FirstName = FirstName.Trim(),
                    LastName = LastName.Trim(),
                    Phone = Phone.Trim(),
                    Address = Address.Trim(),
                };

             
                var validationResult = await _validator.ValidateAsync(registerDto);
                if (!validationResult.IsValid)
                {
                    _errorsList = validationResult.Errors
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return;
                }

                var (success, errorMessage) = await _userService.RegisterServiceCustomer(registerDto);

                if (!success)
                {
                    _errorsList.Add(errorMessage);
                    return;
                }

                Accepted = true; // OK
                ClearFields();
            }
            catch (Exception ex)
            {
                ValidationErrors.Add($"An unexpected error occurred: {ex.Message}");
            }
            finally
            {
                Loading = false;

                if (_errorsList.Count > 0)
                foreach (var error in _errorsList) /// display errors
                {
                    ValidationErrors.Add(error);
                }
                
            }
        }

        [RelayCommand]
        private void GoBackBeginPage()
        {
            App.CustomerNavigationService.GoBack();
        }

        private void ClearFields()
        {
            Email = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Phone = string.Empty;
            Address = string.Empty;
            ValidationErrors.Clear();
        }
    }
}