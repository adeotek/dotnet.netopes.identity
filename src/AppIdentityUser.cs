using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Netopes.Identity
{
    public class AppIdentityUser : IdentityUser<Guid>
    {
        [PersonalData]
        [MaxLength(100)]
        [Required]
        public string FirstName { get; set; }
        [PersonalData]
        [MaxLength(100)]
        [Required]
        public string LastName { get; set; }
        [MaxLength(10)]
        public string CultureInfo { get; set; }
        public int State { get; set; } = 1;
        public int DebugMode { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
