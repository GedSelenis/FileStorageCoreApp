using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class FileAddRequest
    {
        [Required(ErrorMessage = "FileName cannot be blank")]
        public string FileName { get; set; }
        [Required(ErrorMessage = "FilePath cannot be blank")]
        public string FilePath { get; set; }
        public Guid? VirualFolderId { get; set; }
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
            return new FileDetails(FileName, FilePath, VirualFolderId.Value);
        }
    }
}
