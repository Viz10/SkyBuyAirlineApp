using Microsoft.EntityFrameworkCore;
using SkyBuy.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SkyBuy.Data.Model.Entities
{
   
    public class Profile
    {
        [Key]
        public int Id { get; set; }
        public Guid PublicId { get; set; } = Guid.NewGuid();
        
        [MaxLength(100)]    
        public string FirstName { get; set; } = null!;
        
        [MaxLength(100)]
        public string LastName { get; set; } = null!;
                
        [MaxLength(20)]
        public string Phone { get; set; } = null!;

        [MaxLength(250)]
        public string Address { get; set; } = null!;

        public bool IsDeleted { get; set; } = false;


        public int AccountId { get; set; } // FK to customer account 1:1
        public Account Account { get; set; } = null!;
    }
}
