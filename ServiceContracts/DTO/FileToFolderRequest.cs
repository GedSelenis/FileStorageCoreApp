using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class FileToFolderRequest
    {
        public Guid Id { get; set; }
        public Guid? VirualFolderId { get; set; }
    }
}
