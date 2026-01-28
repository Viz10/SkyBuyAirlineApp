using Microsoft.EntityFrameworkCore;
using SkyBuy.Data.Data;
using SkyBuy.Data.Model.DTOs;
using SkyBuy.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TimeZoneConverter;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SkyBuy.Services
{
    public class AirportsClient
    {

        private readonly IDbContextFactory<SkyBuyContext> _contextFactory;
        private readonly HttpClient _httpClient;

        // European country ISO codes
        private static readonly HashSet<string> EuropeanCountries =
        [
            "RO", "GB", "FR", "DE", "IT", "ES", "PT", "NL", "BE", "CH", "AT",
            "PL", "CZ", "SK", "HU", "GR", "BG", "HR", "SI", "RS", "AL", "BA",
            "IE", "DK", "SE", "NO", "FI", "IS", "EE", "LV", "LT", "UA", "BY",
            "MD", "LU", "MT", "CY", "MK", "ME", "XK"
        ];

        public AirportsClient(IDbContextFactory<SkyBuyContext> contextFactory, HttpClient httpClient)
        {
            _contextFactory = contextFactory;
            _httpClient = httpClient;
        }

        public async Task LoadAirportsFromGitHub()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<Dictionary<string,AirportJsonDTO>>(""); /// airports jsons directly serialized

                if (response == null || response.Count == 0)
                {
                    Debug.WriteLine("Did not get jsons");
                    return;
                }

                /// map only european
                var Airports = response
                .Where(pair => EuropeanCountries.Contains(pair.Value.country))
                .Select(pair =>
                {
                    var dto = pair.Value;

                    return new Airport
                    {
                        ICAO = dto.icao.ToUpper(),
                        IATA = string.IsNullOrEmpty(dto.iata) ? null : dto.iata.Trim().ToUpper(),
                        Name = dto.name,
                        City = string.IsNullOrEmpty(dto.city) ? null : dto.city, 
                        CountryISO = dto.country,
                        Country = GetCountryName(dto.country),
                        Lat = dto.lat,
                        Long = dto.lon,
                        Elevation = dto.elevation,
                        Timezone = dto.tz,
                        IsActive = true,
                        Region = string.IsNullOrEmpty(dto.state) ? null : dto.state,
                        IsInternational = !string.IsNullOrEmpty(dto.iata)
                    };

                }).ToList();

                using var db = _contextFactory.CreateDbContext();

                var existingIcaos = await db.Airports
                    .Select(a => a.ICAO)
                    .ToHashSetAsync();

                var newAirports = Airports
                    .Where(a => !existingIcaos.Contains(a.ICAO))
                    .ToList(); /// add new ones

                await db.Airports.AddRangeAsync(newAirports);
                await db.SaveChangesAsync();

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


        public async Task<int?> GetSeatsNumber(string AircraftTypeICAO, string type_of_seats)
        {

            using var db = _contextFactory.CreateDbContext();
            var query = db.AircraftTypes.AsQueryable();
         
            if (type_of_seats == "TypicalSeats")
            {
                query = query.Where(el => el.ICAO == AircraftTypeICAO);
                return await query.Select(a => a.TypicalSeats).FirstOrDefaultAsync();
            }
            else if (type_of_seats == "MaxSeats")
            {
                query = query.Where(el => el.ICAO == AircraftTypeICAO);
                return await query.Select(a => a.MaxSeats).FirstOrDefaultAsync();
            }

            return null;

        }
        public async Task<string?> AirportCodeFromCodeAsync(string code,string value) {

            using var db = _contextFactory.CreateDbContext();
            var query = db.Airports.AsQueryable();

            if (code == "IATA")
            {
                query = query.Where(e => e.IATA == value.ToUpper());
                return await query.Select(e => e.ICAO).FirstOrDefaultAsync();
            }
            else if (code == "ICAO")
            {
                query = query.Where(e => e.ICAO == value.ToUpper());
                return await query.Select(e => e.IATA).FirstOrDefaultAsync();
            }
            return null;
        }
        public async Task<string?> GetTimezoneByCodeAsync(string codeType, string codeValue)
        {

            using var db = _contextFactory.CreateDbContext();
            var query = db.Airports.AsQueryable();

            //logic based on the type requested
            if (codeType.ToLower() == "iata")
            {
                query = query.Where(a => a.IATA == codeValue.ToUpper());
            }
            else if (codeType.ToLower() == "icao")
            {
                query = query.Where(a => a.ICAO == codeValue.ToUpper());
            }

            return await query.Select(a => a.Timezone).FirstOrDefaultAsync();

        }
        private string GetCountryName(string countryISO)
        {
            try
            {
                var region = new RegionInfo(countryISO);
                return region.EnglishName;
            }
            catch
            {
                return countryISO;
            }
        }
    }
}
