using Microsoft.EntityFrameworkCore;
using SkyBuy.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SkyBuy.Data.Model.Entities
{

    public enum AccountType
    {
        Customer = 1,
        Admin = 2,
    }
    public class Account
    {
        [Key]
        public int Id { get; set; }

        public Guid PublicId { get; set; } = Guid.NewGuid();

        [MaxLength(255)]
        public string PasswordHashed { get; set; } = null!;

        [EmailAddress, MaxLength(100)]
        public string? Email { get; set; } // can be null if admin login
       
        [MaxLength(100)]
        public string? Username { get; set; } // can be null if customer login
        
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset ModifiedTime { get; set; } = DateTimeOffset.UtcNow;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTimeOffset? DeletedAt { get; set; }

        public AccountType AccountType { get; set; } 

        public Profile? Profile { get; set; } // link with profile table only when customer 1:1
    }
}
