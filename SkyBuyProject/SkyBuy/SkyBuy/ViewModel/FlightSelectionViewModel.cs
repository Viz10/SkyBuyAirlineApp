using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml; 
using SkyBuy.Data.Data;
using SkyBuy.Data.Model.DTOs;
using SkyBuy.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SkyBuy.ViewModel
{
    public partial class FlightSelectionViewModel : ObservableObject
    {
        private readonly IDbContextFactory<SkyBuyContext> _contextFactory;
        private int _currentAccountId;
        private string _originIATA;
        private string _destinationIATA;
        private DateOnly _departureDate;
        private DateOnly? _returnDate;
        private bool _isRoundTrip;

        public FlightSelectionViewModel(IDbContextFactory<SkyBuyContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        [ObservableProperty] public partial bool IsLoading { get; set; }

        public Visibility ReturnFlightVisibility => _isRoundTrip ? Visibility.Visible : Visibility.Collapsed;

        /// Available Flights
        public ObservableCollection<FlightDisplayDTO> OutboundFlights { get; } = new();
        public ObservableCollection<FlightDisplayDTO> ReturnFlights { get; } = new();

        /// Selected Flights
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanProceed), nameof(TotalPriceDisplay))]
        public partial FlightDisplayDTO? SelectedOutboundFlight { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanProceed), nameof(TotalPriceDisplay))]
        public partial FlightDisplayDTO? SelectedReturnFlight { get; set; }

       
        [ObservableProperty] public partial DateTimeOffset DateOfBirth { get; set; } = DateTimeOffset.Now.AddYears(-25);
        [ObservableProperty][NotifyPropertyChangedFor(nameof(CanProceed))] public partial string PassportNumber { get; set; } = string.Empty;
        [ObservableProperty][NotifyPropertyChangedFor(nameof(CanProceed))] public partial string Nationality { get; set; } = "USA";

      
        [ObservableProperty] public partial string FirstName { get; set; }
        [ObservableProperty] public partial string LastName { get; set; }
        [ObservableProperty] public partial string Email { get; set; }
        [ObservableProperty] public partial string Phone { get; set; }

        
        public string TotalPriceDisplay => $"${((SelectedOutboundFlight?.Price ?? 0) + (SelectedReturnFlight?.Price ?? 0)):F2}";

        public bool CanProceed => SelectedOutboundFlight != null &&
                                  (!_isRoundTrip || SelectedReturnFlight != null) &&
                                  !string.IsNullOrWhiteSpace(PassportNumber) &&
                                  !string.IsNullOrWhiteSpace(Nationality);

        public async void receiveData(FlightSelectDatasDTO data)
        {
            Debug.WriteLine($"receiveData called. AccountId: {data.AccountId}, Origin: {data.OriginIATA}, Dest: {data.DestinationIATA}");
            Debug.WriteLine($"Dates: Dep={data.DepartureDate}, Ret={data.ReturnDate}, IsRoundTrip={data.IsRoundTrip}");

            _currentAccountId = data.AccountId;
            _originIATA = data.OriginIATA;
            _destinationIATA = data.DestinationIATA;
            _departureDate = data.DepartureDate;
            _returnDate = data.ReturnDate;
            _isRoundTrip = data.IsRoundTrip;

            OnPropertyChanged(nameof(ReturnFlightVisibility));

            await LoadUserProfileDataAsync();
            await LoadAvailableFlightsAsync();
        }

        public async Task LoadUserProfileDataAsync()
        {
            try
            {
                using var db = await _contextFactory.CreateDbContextAsync();
                Debug.WriteLine($"Searching for Account ID: {_currentAccountId}");

                var account = await db.Accounts
                    .Include(a => a.Profile)
                    .FirstOrDefaultAsync(a => a.Id == _currentAccountId);

                if (account == null)
                {
                    Debug.WriteLine("Account not found in Database.");
                    return;
                }

                if (account.Profile != null)
                {
                    FirstName = account.Profile.FirstName;
                    LastName = account.Profile.LastName;
                    Email = account.Email ?? string.Empty;
                    Phone = account.Profile.Phone;
                    Debug.WriteLine($"Profile loaded: {FirstName} {LastName}");
                }
                else
                {
                    Debug.WriteLine("Account found but Profile is NULL.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR in LoadUserProfileDataAsync: {ex.Message}");
            }
        }

        public async Task LoadAvailableFlightsAsync()
        {
            try
            {
                IsLoading = true;
                OutboundFlights.Clear();
                ReturnFlights.Clear();

                using var db = await _contextFactory.CreateDbContextAsync();
                Debug.WriteLine($"Querying Route: {_originIATA} -> {_destinationIATA}");

                ///DateOnly to DateTime range for database query
                var departureDateStart = _departureDate.ToDateTime(TimeOnly.MinValue);
                var departureDateEnd = _departureDate.ToDateTime(TimeOnly.MaxValue);

                var outboundFlights = await db.Flights
                    .Where(f => f.FlightSchedule.FlightService.Route.Origin.IATA == _originIATA &&
                                f.FlightSchedule.FlightService.Route.Destination.IATA == _destinationIATA &&
                                f.DepartureDateTimeUTC >= departureDateStart &&
                                f.DepartureDateTimeUTC <= departureDateEnd)
                    .Select(f => new FlightDisplayDTO
                    {
                        FlightId = f.Id,
                        FlightNumber = f.FlightSchedule.FlightService.FlightIATA ?? "N/A",
                        Price = f.CurrentPrice,
                        DisplayText = $"{f.FlightSchedule.FlightService.FlightIATA} | {f.DepartureDateTimeUTC:HH:mm} | ${f.CurrentPrice:F2}"
                    })
                    .ToListAsync();

                Debug.WriteLine($"Found {outboundFlights.Count} outbound flights for {_departureDate}.");

                foreach (var f in outboundFlights)
                {
                    OutboundFlights.Add(f);
                }

                /// Handle Return Flights
                if (_isRoundTrip && _returnDate.HasValue)
                {
                    Debug.WriteLine($"Return Route: {_destinationIATA} -> {_originIATA}");

                    var returnDateStart = _returnDate.Value.ToDateTime(TimeOnly.MinValue);
                    var returnDateEnd = _returnDate.Value.ToDateTime(TimeOnly.MaxValue);

                    var returnFlights = await db.Flights
                        .Where(f => f.FlightSchedule.FlightService.Route.Origin.IATA == _destinationIATA &&
                                    f.FlightSchedule.FlightService.Route.Destination.IATA == _originIATA &&
                                    f.DepartureDateTimeUTC >= returnDateStart &&
                                    f.DepartureDateTimeUTC <= returnDateEnd)
                        .Select(f => new FlightDisplayDTO
                        {
                            FlightId = f.Id,
                            FlightNumber = f.FlightSchedule.FlightService.FlightIATA ?? "N/A",
                            Price = f.CurrentPrice,
                            DisplayText = $"{f.FlightSchedule.FlightService.FlightIATA} | {f.DepartureDateTimeUTC:HH:mm} | ${f.CurrentPrice:F2}"
                        })
                        .ToListAsync();

                    Debug.WriteLine($"Found {returnFlights.Count} return flights for {_returnDate.Value}.");

                    foreach (var f in returnFlights)
                    {
                        ReturnFlights.Add(f);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadAvailableFlightsAsync: {ex.Message}");
                if (ex.InnerException != null) Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task ProceedToBookingAsync()
        {
            if (!CanProceed || SelectedOutboundFlight == null) return;

            try
            {
                IsLoading = true;
                using var db = await _contextFactory.CreateDbContextAsync();

                var newBooking = new Booking
                {
                    BookingReference = GenerateBookingReference(),
                    FlightId = SelectedOutboundFlight.FlightId,
                    AccountId = _currentAccountId,
                    NumberOfPassengers = 1,
                    TotalPrice = SelectedOutboundFlight.Price,
                    Status = BookingStatus.Confirmed,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Flight = null!,
                    Account = null!
                };

                var passenger = new Passenger
                {
                    Booking = newBooking,
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Phone = Phone,
                    PassportNumber = PassportNumber,
                    Nationality = Nationality,
                    DateOfBirth = DateOnly.FromDateTime(DateOfBirth.Date),
                    PersonGender = Gender.Male,
                    Type = PassengerType.Adult
                };

                db.Passengers.Add(passenger);

                await db.SaveChangesAsync();

                Debug.WriteLine($"Booking Successful! Ref: {newBooking.BookingReference}");

                App.CustomerNavigationService.GoBack();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Booking Error: {ex.Message}");
            }
            finally { IsLoading = false; }
        }

        private string GenerateBookingReference()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [RelayCommand]
        public void GoBack() => App.CustomerNavigationService.GoBack();
    }

    public class FlightDisplayDTO
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public decimal Price { get; set; }
        public string DisplayText { get; set; }
    }
}