using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        [StringLength(256)]
        public string? UserName { get; set; }
        [StringLength(256)]
        public string? Email { get; set; }
        [StringLength(256)]
        public string? NormalizedUserName { get; set; }
        [StringLength(256)]
        public string? NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        [StringLength(450)]
        public Guid? SecretUserId { get; set; }

        [ForeignKey("SecretUserId")]
        public SecretUser? SecretUser { get; set; }

        public IdentityUser ToIdentityUser()
        {
            return new IdentityUser
            {
                Id = UserId.ToString(),
                UserName = UserName,
                Email = Email,
                NormalizedUserName = NormalizedUserName,
                NormalizedEmail = NormalizedEmail,
                PasswordHash = SecretUser?.PasswordHash,
            };
        }
    }
    public static class IdentityUserExtensions
    {
        public static User ToUser(this IdentityUser identityUser)
        {
            return new User
            {
                UserId = Guid.Parse(identityUser.Id),
                UserName = identityUser.UserName,
                Email = identityUser.Email,
                NormalizedEmail = identityUser.NormalizedEmail,
                NormalizedUserName = identityUser.NormalizedUserName
            };
        }
    }
}
