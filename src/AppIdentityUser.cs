using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Netopes.Identity
{
    public class AppIdentityUser : IdentityUser<Guid>
    {
        [PersonalData]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [PersonalData]
        [MaxLength(100)]
        public string LastName { get; set; }
        [MaxLength(10)]
        public string CultureInfo { get; set; }
        public int State { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
