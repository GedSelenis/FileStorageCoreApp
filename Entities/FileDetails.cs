using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class FileDetails
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Guid? VirualFolderId { get; set; }

        public FileDetails(string fileName, string filePath, Guid virualFolder)
        {
            this.FileName = fileName;
            this.FilePath = filePath;
            this.VirualFolderId = virualFolder;
            this.Id = Guid.NewGuid();
        }
        public FileDetails(Guid id, string fileName, string filePath)
        {
            this.FileName = fileName;
            this.FilePath = filePath;
            this.Id = id;
        }
        public FileDetails(Guid id, string fileName, string filePath, Guid virualFolder)
        {
            this.FileName = fileName;
            this.FilePath = filePath;
            this.Id = id;
            this.VirualFolderId = virualFolder;
        }
        public FileDetails()
        {
            FileName = string.Empty;
            FilePath = string.Empty;
        }
    }
}
