using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using SkyBuy.Data.Data;
using SkyBuy.Data.Model.DTOs;
using SkyBuy.Data.Model.Entities;
using SkyBuy.Models;
using System.Data.Common;
using System.Security.Cryptography;

namespace SkyBuy.Services
{
    public class AuthenticationService
    {
        private readonly SkyBuyContext _dbContext; 
        public AuthenticationService(SkyBuyContext dbContext) { _dbContext = dbContext; } // DI
        


        async public Task<LoginResultDTO> LoginService(LoginCredentialsDTO loginDataDTO)
        {

            try
            {
                /// first get account by unique field (email/username) , if customer eagerly load profile data also with a SQL join
                var account = (loginDataDTO.IsAdmin) ?
                await _dbContext.Accounts.SingleOrDefaultAsync(acc => acc.Username == loginDataDTO.AccountUniqueCredential && acc.AccountType == AccountType.Admin)
                                                     :
                await _dbContext.Accounts.Include(acc => acc.Profile).SingleOrDefaultAsync(acc => acc.Email == loginDataDTO.AccountUniqueCredential && acc.AccountType == AccountType.Customer); ///eagger loading profile

                                                                                                                                                                                                 /// account does not exist
                if (account == null) { return new LoginResultDTO { Success = false, Error = "Invalid Credentials", AccountData = null }; }

                // if it exists check password
                if (!VerifyPassword(account.PasswordHashed, loginDataDTO.Password)) { return new LoginResultDTO { Success = false, Error = "Invalid Credentials", AccountData = null }; }

                /// check its status
                if (!account.IsActive) { return new LoginResultDTO { Success = false, Error = "Account has been blocked. Contact system admin for support.", AccountData = null }; }

                /// return data to ModelView
                if (account.Profile != null)
                {
                    return new LoginResultDTO { Success = true, Error = null, AccountData = new AccountWelcomeDataDTO { Id = account.Id, Type = account.AccountType, Name = $"{account.Profile.FirstName} {account.Profile.LastName}", Username = null } };
                }
                else
                {
                    return new LoginResultDTO { Success = true, Error = null, AccountData = new AccountWelcomeDataDTO { Id = account.Id, Type = account.AccountType, Name = null, Username = account.Username } };
                }
            }
            catch (DbException)
            {
                return new LoginResultDTO
                {
                    Success = false,
                    Error = "Database connection error. Please try again.",
                    AccountData = null
                };
            }
            catch (Exception)
            {
                return new LoginResultDTO
                {
                    Success = false,
                    Error = "Login failed. Please try again.",
                    AccountData = null
                };
            }
        }
        
        
        
        async public Task<(bool,string?)> RegisterServiceCustomer(RegisterFormCustomerDTO registerDto)
        {
            try
            {
                bool ok = await _dbContext.Accounts.AnyAsync(acc => acc.Email == registerDto.Email); /// null at email rows will be skipped ,  but as already index it has contraint to include to index setonly the ones not null

                if (ok)
                {
                    return (false,"Account with this email already exists!");
                }

                Account account = new Account
                {
                    PasswordHashed = HashPassword(registerDto.Password),
                    Email = registerDto.Email,
                    AccountType = AccountType.Customer
                };

                Profile profile = new Profile
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Phone = registerDto.Phone,
                    Address = registerDto.Address,
                    Account = account /// link in ef core
                };

                _dbContext.Profiles.Add(profile); /// add both
                await _dbContext.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                return (false, $"Registration failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }
        async public Task<(bool, string?)> RegisterServiceAdmin(RegisterFormAdminDTO registerDto)
        {

            bool ok = await _dbContext.Accounts.AnyAsync(acc => acc.Username == registerDto.Username);

            if (ok)
            {
                return (false, "Admin account with this username already exists!");
            }

            Account account = new Account
            {
                PasswordHashed = HashPassword(registerDto.Password),
                Username = registerDto.Username,
                AccountType = AccountType.Admin
            };

            try
            {
                _dbContext.Accounts.Add(account);
                await _dbContext.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                return (false, $"Registration failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }


        private string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }
        private bool VerifyPassword(string storedHash, string providedPassword)
        {
            var parts = storedHash.Split('.');
            var salt = Convert.FromBase64String(parts[0]);
            var hash = parts[1];

            string hashedProvided = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: providedPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hash == hashedProvided;
        }
    }
}
