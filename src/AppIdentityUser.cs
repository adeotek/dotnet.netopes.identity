using System;
using Microsoft.AspNetCore.Identity;

namespace Netopes.Identity
{
    public class AppIdentityUser : IdentityUser<Guid>
    {
        // Shared data
        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
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
        public int EntityState { get; set; }
        // User specific data
        public string CultureInfo { get; set; }
        public int State { get; set; }
        public int DebugMode { get; set; }
        public int IsMasterAccount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LockoutEndForDb => LockoutEnd?.DateTime;

        public AppIdentityUser()
        {
            Id = Guid.NewGuid();
            EntityState = -1;
            State = -1;
            DebugMode = 0;
            IsMasterAccount = 0;
            CreatedAt = DateTime.Now;
        }
    }
}
