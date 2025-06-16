using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class FileAddRequest
    { 
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string VirtualFolder { get; set; }
        public FileAddRequest(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
        }
        public FileAddRequest()
        {
            FileName = string.Empty;
            FilePath = string.Empty;
        }

        public FileDetails ToFileDetails()
        {
            return new FileDetails(FileName, FilePath, VirtualFolder);
        }
    }
}
