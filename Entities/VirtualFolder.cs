using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class VirtualFolder
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? ParentFolderId { get; set; }
        [StringLength(40)]
        public string? FolderName { get; set; }
        [ForeignKey("ParentFolderId")]
        public VirtualFolder? ParentFolder { get; set; }
    }
}
