using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SkyBuy.Data.Data;
using SkyBuy.Data.Model.DTOs;
using SkyBuy.Data.Model.Entities;
using SkyBuy.Models;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace SkyBuy.Services
{
    
    /// the object retrieved from AirLabs , list of json named Response
    public class AirLabsFleetResponse 
    {
        [JsonPropertyName("response")]
        public List<AircraftJsonDTO>? Response { get; set; }
    }
    public class AirLabsRouteResponse 
    {
        [JsonPropertyName("response")]
        public List<PartialScheduleJsonDTO>? Response { get; set; }
    }


    public class AirLabsClient
    {
        
        private readonly SkyBuyContext _dbContext; /// scoped service
            
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AirLabsClient(SkyBuyContext dbContext, HttpClient httpClient,IConfiguration configuration)
        {
            _dbContext = dbContext;
            _httpClient = httpClient;
            _apiKey = configuration["Airlabs:ApiKey"] ?? throw new InvalidOperationException("ApiKey error");
        }


        public async Task<RouteResultDTO> LoadRouteAsync(FlightRouteParametersDTO data)
        {
            try
            {

                var response = await _httpClient.GetFromJsonAsync<AirLabsRouteResponse>
                    ($"routes?api_key={_apiKey}" +
                    $"&dep_iata={data.DepartureAirport_IATA}" +
                    $"&arr_iata={data.ArrivalAirport_IATA}" +
                    $"&airline_iata={data.Airline_IATA}");

               if ( response != null ) {
               
                    var routes = response.Response;

                    /// route does not exit IRL, manually create one
                    if (routes == null || routes.Count == 0){
                        Debug.WriteLine("No routes found for given parameters");
                        return new RouteResultDTO() {ErrorMsg=null, Found = false, Routes=null };
                    }

                    var GroupedRouts = routes
                    .GroupBy(r => new { r.AirlineIATA, r.FlightNumber })
                    .Select(g => g.First()) /// first from grouping-value list which all have same 4 data
                    .ToList();

                    /// check if real life route already added by FlightIATA
                    var Present = await _dbContext.FlightSchedules.Include(el=>el.FlightService).Select(element=>element.FlightService.FlightIATA).ToHashSetAsync(); /// W63001
                    var uniqueFlights = GroupedRouts.Where(element=>!Present.Contains(element.FlightIATA)).ToList();

                    if (uniqueFlights.IsNullOrEmpty()) { return new RouteResultDTO() {Found = false}; } /// all are added , signal to create randomly

                    return new RouteResultDTO() { Found = true, Routes = uniqueFlights };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                if (ex.InnerException != null)
                {
                    Debug.WriteLine("INNER: " + ex.InnerException.Message);
                }
            }
            return new RouteResultDTO { Found = false, ErrorMsg = "Loading Route error" };
        }

        
        /// called once to init fleet
        public async Task LoadAircraftsAsync(string airline_iata) /// W4 OR W6
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"fleets?api_key={_apiKey}&airline_iata={airline_iata}");
                
                var result = JsonSerializer.Deserialize<AirLabsFleetResponse>(response);

                if (result == null)
                {
                    Debug.WriteLine("Could not parse jsons");
                    return;
                }

                var fleet = result.Response;

                if (fleet == null || fleet.Count == 0)
                {
                    Debug.WriteLine("Did not get jsons");
                    return;
                }
                
                
                /// map 
                var Airplanes = fleet
                .Where(item=>item.icao!=null)
                .Select(item =>
                {
                    var type = GetAircraftType(item.icao) ?? throw new InvalidOperationException("Airplane type not found");
                    var airline = GetAirline(item.airline_icao) ?? throw new InvalidOperationException("Airline not found");

                    return new FleetAircraft
                    {
                        RegistrationNumber = item.hex,
                        AircraftType = type,
                        Airline = airline,
                        AircraftTypeICAO = type.ICAO
                    };
                }
                   
                ).ToList();


                var existing = await _dbContext.FleetAircrafts
                .Select(a => a.RegistrationNumber)
                .ToHashSetAsync();

                var newAircrafts = Airplanes
                .Where(a => !existing.Contains(a.RegistrationNumber))
                .ToList();

                await _dbContext.FleetAircrafts.AddRangeAsync(newAircrafts);
                await _dbContext.SaveChangesAsync();
                
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine(ex.InnerException?.Message);
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                if (ex.InnerException != null)
                {
                    Debug.WriteLine("INNER: " + ex.InnerException.Message);
                }
            }
        }
        private  AircraftType? GetAircraftType(string ICAO)
        {
            return _dbContext.AircraftTypes.Find(ICAO); /// return reference
        }
        private Airline? GetAirline(string ICAO)
        {
            return _dbContext.Airlines.Find(ICAO);
        }
        public async Task<HashSet<string>> GetAirplaneModelsAsync(string airline_iata) {
            return await _dbContext.FleetAircrafts.Where(a => a.Airline != null && a.Airline.IATA== airline_iata)
                                                  .Select(a => a.AircraftType.ICAO)
                                                  .Distinct()
                                                  .ToHashSetAsync();
        }                                                                
    }
}
