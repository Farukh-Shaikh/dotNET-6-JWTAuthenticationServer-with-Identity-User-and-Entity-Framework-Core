using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace JWTAuthServer.Common.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Status { get; set; }
        public int? Tenant_ID { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool FirstPasswordChange { get; set; }
        public bool Agreement_Flag { get; set; }
        public DateTime? Last_Password_Reset_Date { get; set; }
        [NotMapped]
        public string Password { get; set; }
    }
}
