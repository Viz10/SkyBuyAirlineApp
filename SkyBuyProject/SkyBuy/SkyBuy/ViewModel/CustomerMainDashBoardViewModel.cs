using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SkyBuy.Data.Data;
using SkyBuy.Data.Model.DTOs;
using SkyBuy.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace SkyBuy.ViewModel
{
    public partial class CustomerMainDashBoardViewModel : ObservableObject
    {
        private readonly IDbContextFactory<SkyBuyContext> _contextFactory;
        private AccountWelcomeDataDTO _welcomeData;

        public CustomerMainDashBoardViewModel(IDbContextFactory<SkyBuyContext> contextFactory)
        {
            _contextFactory = contextFactory;

            // Initialize dates - set range dynamically based on available flights
            InitializeDateRange();

            // Load airports from database
            _ = LoadAirportsAsync();
        }

        [ObservableProperty]
        public partial bool emptyBooking { get; set; } = false;


        private int AccId { get; set; }

        [ObservableProperty]
        public partial string WelcomeName { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TwoWaySelected))]
        public partial int SelectedIndex { get; set; } = 0;

        [ObservableProperty]
        public partial DateTimeOffset MinDate { get; set; }

        [ObservableProperty]
        public partial DateTimeOffset MaxDate { get; set; }

        [ObservableProperty]
        public partial DateTimeOffset FirstDate { get; set; }

        [ObservableProperty]
        public partial DateTimeOffset SecondDate { get; set; }

        [ObservableProperty]
        public partial string Test { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSearch))]
        public partial bool IsLoading { get; set; }

        public HashSet<DateTimeOffset> BlockedDates { get; set; } = new HashSet<DateTimeOffset>();

        public bool TwoWaySelected => (SelectedIndex == 0);

    
        [ObservableProperty]
        public partial ObservableCollection<string> DepartureSuggestions { get; set; } = new ObservableCollection<string>();

        [ObservableProperty]
        public partial ObservableCollection<string> ArrivalSuggestions { get; set; } = new ObservableCollection<string>();

      
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSearch))]
        public partial string DepartureCity { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSearch))]
        public partial string ArrivalCity { get; set; }

   
        ///(City Name - IATA Code)
        public List<AirportDisplayDTO> Airports { get; set; } = new List<AirportDisplayDTO>();

       
        private Dictionary<string, string> _cityToIATA = new Dictionary<string, string>();

        public bool CanSearch => !string.IsNullOrWhiteSpace(DepartureCity) &&
                                 !string.IsNullOrWhiteSpace(ArrivalCity) &&
                                 DepartureCity != "Nothing found" &&
                                 ArrivalCity != "Nothing found" &&
                                 !IsLoading;

        

        ///Initialize the date range to show whole year
        private void InitializeDateRange()
        {
            MinDate = new DateTimeOffset(
                DateTimeOffset.Now.Year,
                DateTimeOffset.Now.Month,
                DateTimeOffset.Now.Day,
                0, 0, 0,
                DateTimeOffset.Now.Offset);

            MaxDate = DateTimeOffset.Now.AddYears(1); 
            FirstDate = MinDate;
            SecondDate = MinDate.AddDays(1);
        }
        ///Load all airports from database that have active flight schedules
        private async Task LoadAirportsAsync()
        {
            try
            {
                IsLoading = true;

                using var db = await _contextFactory.CreateDbContextAsync();

                var originAirports = await db.FlightSchedules
                    .Where(fs => fs.ValidUntil >= DateOnly.FromDateTime(DateTime.Now))
                    .Select(fs => fs.FlightService.Route.OriginICAO)
                    .Distinct()
                    .ToListAsync();

                var destAirports = await db.FlightSchedules
                    .Where(fs => fs.ValidUntil >= DateOnly.FromDateTime(DateTime.Now))
                    .Select(fs => fs.FlightService.Route.DestinationICAO)
                    .Distinct()
                    .ToListAsync();

                var allAirportICAOs = originAirports.Union(destAirports).Distinct().ToList();

                var airports = await db.Airports
                    .Where(a => allAirportICAOs.Contains(a.ICAO))
                    .Select(a => new AirportDisplayDTO
                    {
                        IATA = a.IATA,
                        ICAO = a.ICAO,
                        CityName = a.City,
                        CountryName = a.Country,
                        DisplayName = $"{a.City} ({a.IATA})"
                    })
                    .OrderBy(a => a.CityName)
                    .ToListAsync();

                Airports = airports;

              
                _cityToIATA.Clear();
                foreach (var airport in airports)
                {
                    _cityToIATA[airport.DisplayName] = airport.IATA;
                }

                Debug.WriteLine($" Loaded {Airports.Count} airports with active flights");

            
                await UpdateDateRangeFromFlightsAsync(db);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($" Error loading airports: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        ///Update MinDate and MaxDate based on actual flight availability
        private async Task UpdateDateRangeFromFlightsAsync(SkyBuyContext db)
        {
            try
            {
                var earliestFlight = await db.FlightSchedules
                    .Where(fs => fs.ValidFrom >= DateOnly.FromDateTime(DateTime.Now))
                    .OrderBy(fs => fs.ValidFrom)
                    .Select(fs => fs.ValidFrom)
                    .FirstOrDefaultAsync();

                var latestFlight = await db.FlightSchedules
                    .OrderByDescending(fs => fs.ValidUntil)
                    .Select(fs => fs.ValidUntil)
                    .FirstOrDefaultAsync();

                if (earliestFlight != default && latestFlight != default)
                {
                    MinDate = new DateTimeOffset(earliestFlight.ToDateTime(TimeOnly.MinValue), DateTimeOffset.Now.Offset);
                    MaxDate = new DateTimeOffset(latestFlight.ToDateTime(TimeOnly.MinValue), DateTimeOffset.Now.Offset);

                    if (FirstDate < MinDate)
                        FirstDate = MinDate;

                    Debug.WriteLine($" Date range: {MinDate:yyyy-MM-dd} to {MaxDate:yyyy-MM-dd}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($" Error updating date range: {ex.Message}");
            }
        }


        [ObservableProperty]
        public partial ObservableCollection<BookingDisplayDTO> MyBookings { get; set; } = new();

        ///load user bookings
        public async Task LoadUserBookingsAsync()
        {
            try
            {
                emptyBooking = false;

                using var db = await _contextFactory.CreateDbContextAsync();

                var bookings = await db.Bookings
                     .Where(b => b.AccountId == AccId)
                     .OrderByDescending(b => b.CreatedAt)
                     .Select(b => new BookingDisplayDTO
                     {
                         Reference = b.BookingReference,
                         Route = $"{b.Flight.FlightSchedule.FlightService.Route.OriginICAO} -> {b.Flight.FlightSchedule.FlightService.Route.DestinationICAO}",
                         DepartureTime = b.Flight.DepartureDateTimeUTC.ToString("g"),
                         Status = b.Status.ToString(),
                         Price = b.TotalPrice
                     })
                     .ToListAsync();

                MyBookings.Clear();
                foreach (var b in bookings) MyBookings.Add(b);

                if(MyBookings.Count == 0)
                {
                    emptyBooking=true;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DEBUG] Error loading bookings: {ex.Message}");
            }
        }

        [RelayCommand]
        internal void TextChangedDeparture(string text)
        {
            DepartureSuggestions.Clear();

            if (string.IsNullOrWhiteSpace(text))
                return;

            var matchingAirports = Airports
                .Where(a => a.DisplayName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                           a.CityName.StartsWith(text, StringComparison.OrdinalIgnoreCase) ||
                           a.IATA.StartsWith(text, StringComparison.OrdinalIgnoreCase))
                .OrderBy(a => a.CityName)
                .Take(10)
                .ToList();

            if (matchingAirports.Count == 0)
            {
                DepartureSuggestions.Add("Nothing found");
                return;
            }

            foreach (var airport in matchingAirports)
            {
                DepartureSuggestions.Add(airport.DisplayName);
            }

            Debug.WriteLine($"Departure search: '{text}' - Found {matchingAirports.Count} matches");
        }

        [RelayCommand]
        internal void TextChangedArrival(string text)
        {
            ArrivalSuggestions.Clear();

            if (string.IsNullOrWhiteSpace(text))
                return;

            var matchingAirports = Airports
                .Where(a => a.DisplayName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                           a.CityName.StartsWith(text, StringComparison.OrdinalIgnoreCase) ||
                           a.IATA.StartsWith(text, StringComparison.OrdinalIgnoreCase))
                .Where(a => !TwoWaySelected || !string.Equals(a.DisplayName, DepartureCity, StringComparison.OrdinalIgnoreCase))
                .OrderBy(a => a.CityName)
                .Take(10)
                .ToList();

            if (matchingAirports.Count == 0)
            {
                ArrivalSuggestions.Add("Nothing found");
                return;
            }

            foreach (var airport in matchingAirports)
            {
                ArrivalSuggestions.Add(airport.DisplayName);
            }
        }

        [RelayCommand]
        internal async Task SuggestionChosenAsync((string Chosen, int tag) data)
        {
            if (data.tag == 1)
            {
                DepartureCity = data.Chosen;

                if (TwoWaySelected && string.Equals(DepartureCity, ArrivalCity, StringComparison.OrdinalIgnoreCase))
                {
                    ArrivalCity = null;
                    Debug.WriteLine(" Cleared arrival city (same as departure for round-trip)");
                }

                UpdateTestDisplay();
            }
            else if (data.tag == 2)
            {
                if (TwoWaySelected && string.Equals(data.Chosen, DepartureCity, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine(" Cannot select same city for departure and arrival on round-trip");
                    return;
                }

                ArrivalCity = data.Chosen;
                UpdateTestDisplay();
            }

            if (!string.IsNullOrWhiteSpace(DepartureCity) &&
                !string.IsNullOrWhiteSpace(ArrivalCity) &&
                DepartureCity != "Nothing found" &&
                ArrivalCity != "Nothing found")
            {
                await UpdateBlockedDatesAsync();
            }
        }

        [RelayCommand]
        private void ClearSelection()
        {
            DepartureCity = null;
            ArrivalCity = null;
            DepartureSuggestions.Clear();
            ArrivalSuggestions.Clear();
            BlockedDates.Clear();
            Test = "Select cities";
            OnPropertyChanged(nameof(BlockedDates));
        }

        [RelayCommand]
        private async Task Searching()
        {
            if (!CanSearch)
            {
                Debug.WriteLine(" Cannot search - incomplete selection");
                return;
            }

            if (string.Equals(DepartureCity, ArrivalCity, StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine(" Departure and Arrival cities cannot be the same");
                return;
            }

            try
            {
                IsLoading = true;

                _cityToIATA.TryGetValue(DepartureCity, out var depIATA);
                _cityToIATA.TryGetValue(ArrivalCity, out var arrIATA);

                using var db = _contextFactory.CreateDbContext();

                var searchParams = new FlightSelectDatasDTO
                {
                    AccountId= AccId,
                    OriginIATA = depIATA,
                    DestinationIATA = arrIATA,
                    DepartureDate = DateOnly.FromDateTime(FirstDate.Date),
                    ReturnDate = TwoWaySelected ? DateOnly.FromDateTime(SecondDate.Date) : null
                };

                Debug.WriteLine($"Searching: {depIATA} -> {arrIATA} | Outbound: {searchParams.DepartureDate}");

                App.CustomerNavigationService.NavigateTo("FlightSelection", searchParams);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Search error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void GoBackLoginPage()
        {
            App.CustomerNavigationService.GoBack();
        }

        private async Task UpdateBlockedDatesAsync()
        {
            if (string.IsNullOrWhiteSpace(DepartureCity) || string.IsNullOrWhiteSpace(ArrivalCity)) return;

            try
            {
                IsLoading = true;

                var newBlockedDates = new HashSet<DateTimeOffset>();

                _cityToIATA.TryGetValue(DepartureCity, out var depIATA);
                _cityToIATA.TryGetValue(ArrivalCity, out var arrIATA);

                using var db = await _contextFactory.CreateDbContextAsync();

                var availableDates = await db.Flights
                    .Where(f => f.FlightSchedule.FlightService.Route.Origin.IATA == depIATA &&
                                f.FlightSchedule.FlightService.Route.Destination.IATA == arrIATA)
                    .Select(f => f.DepartureDateTimeUTC.Date)
                    .Distinct()
                    .ToListAsync();

                var availableSet = new HashSet<DateTime>(availableDates);

                DateTime start = MinDate.DateTime.Date;
                DateTime end = MaxDate.DateTime.Date;

                for (var day = start; day <= end; day = day.AddDays(1))
                {
                    if (!availableSet.Contains(day))
                    {
                        newBlockedDates.Add(new DateTimeOffset(day, DateTimeOffset.Now.Offset));
                    }
                }

                /// Update the property and Notify the UI
                BlockedDates = newBlockedDates;
                OnPropertyChanged(nameof(BlockedDates));

                Debug.WriteLine($"[DEBUG] Blocked {BlockedDates.Count} dates. Available: {availableSet.Count}");
            }
            finally { IsLoading = false; }
        }


        private void UpdateTestDisplay()
        {
            if (!string.IsNullOrWhiteSpace(DepartureCity) && !string.IsNullOrWhiteSpace(ArrivalCity))
            {
                Test = $"{DepartureCity} → {ArrivalCity}";
            }
            else if (!string.IsNullOrWhiteSpace(DepartureCity))
            {
                Test = $"{DepartureCity} → ?";
            }
            else if (!string.IsNullOrWhiteSpace(ArrivalCity))
            {
                Test = $"? → {ArrivalCity}";
            }
            else
            {
                Test = "Select cities";
            }
        }
        internal void receiveCustomerData(AccountWelcomeDataDTO data)
        {
            _welcomeData = data;
            WelcomeName = data.Name;
            AccId = data.Id;
        }
    }



    ///DTOs 
    public class AirportDisplayDTO
    {
        public string IATA { get; set; }
        public string ICAO { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public string DisplayName { get; set; }
    }

    public class BookingDisplayDTO
    {
        public string Reference { get; set; }
        public string Route { get; set; }
        public string DepartureTime { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public string DisplaySummary => $"{Reference} | {Route} | {DepartureTime} (${Price:F2})";
    }
}