using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class FolderToFolderRequest
    {
        public Guid Id { get; set; }
        public Guid? ParentFolderId { get; set; }
    }
}
