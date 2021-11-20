using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Netopes.Identity
{
    public class AppAccount
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(255)]
        [Required]
        public string Email { get; set; }
        [MaxLength(255)]
        public string Region { get; set; }
        [MaxLength(255)]
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public string AddressDetails { get; set; }
        [MaxLength(100)]
        public string PostalCode { get; set; }
        public int State { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<AppIdentityUserWithAccount> Users { get; set; }
    }
}
