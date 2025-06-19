using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class FolderUpdateRequest
    {
        [Required(ErrorMessage = "Id cannot be blank")]
        public Guid Id { get; set; }
        public string? FolderName { get; set; }

    }
}
