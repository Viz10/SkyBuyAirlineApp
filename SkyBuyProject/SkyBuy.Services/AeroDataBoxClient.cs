using Azure;
using SkyBuy.Data.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace SkyBuy.Services
{

    public class Distance
    {
        [JsonPropertyName("meter")]
        public double Meter { get; set; }

        [JsonPropertyName("km")]
        public double Km { get; set; }

        [JsonPropertyName("mile")]
        public double Mile { get; set; }

        [JsonPropertyName("nm")]
        public double Nm { get; set; }

        [JsonPropertyName("feet")]
        public double Feet { get; set; }
    }

    public class DistanceResponse
    {
        [JsonPropertyName("greatCircleDistance")]
        public Distance Distance { get; set; } = null!;

        [JsonPropertyName("approxFlightTime")]
        public string ApproxFlightTime { get; set; } = null!;
    }

    


    public  class AeroDataBoxClient
    {
        private readonly HttpClient _httpClient;

        public AeroDataBoxClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DistanceTimeResponseDTO> GetDistanceTimeAsync(FlightRouteParametersDTO data)
        {
            try
            {
                var DistanceResponseData = await _httpClient.GetFromJsonAsync<DistanceResponse>($"airports/Iata/{data.DepartureAirport_IATA}/distance-time/{data.ArrivalAirport_IATA}?aircraftName={data.AircraftTypeICAO}&flightTimeModel=Standard");

                if (DistanceResponseData == null) { return new DistanceTimeResponseDTO() { Ok = false, ErrorMsg = "Error fetching Dist Time Resp." }; }

                return new DistanceTimeResponseDTO() { Ok         = true,
                                                       Duration   = DistanceResponseData.ApproxFlightTime, /// 03:25:00
                                                       DistanceKm = DistanceResponseData.Distance.Km /// 5644654.35
                };

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                if (ex.InnerException != null)
                {
                    Debug.WriteLine("INNER: " + ex.InnerException.Message);
                }
                return new DistanceTimeResponseDTO() { Ok = false , ErrorMsg = ex.ToString() };
            }
        }
    }
}
