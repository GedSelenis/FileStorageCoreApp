using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace ServiceContracts.DTO
{
    public class FileRenameRequest
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string NewFileName { get; set; }

        public FileDetails ToFileDetails()
        {
            return new FileDetails(Id, FileName, FilePath);
        }
    }
}
