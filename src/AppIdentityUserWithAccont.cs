using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Netopes.Identity
{
    public class AppIdentityUserWithAccount : IdentityUser<Guid>
    {
        // Shared data
        [PersonalData]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [PersonalData]
        [MaxLength(100)]
        public string LastName { get; set; }
        // Entity specific data
        public Guid AccountId { get; set; }
        public Guid CountryId { get; set; }
        public string CompanyName { get; set; }
        public string TaxCode { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public string AddressDetails { get; set; }
        public string PostalCode { get; set; }

        public int EntityState { get; set; } = -1;
        // User specific data
        [MaxLength(10)]
        public string CultureInfo { get; set; }
        public int State { get; set; } = -1;
        public int DebugMode { get; set; }
        public int IsMasterAccount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LockoutEndForDb => LockoutEnd?.DateTime;
    }
}
