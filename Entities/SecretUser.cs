using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class SecretUser
    {
        [Key]
        public Guid Id { get; set; }
        public string? PasswordHash { get; set; }
        public Guid? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

    }
}
