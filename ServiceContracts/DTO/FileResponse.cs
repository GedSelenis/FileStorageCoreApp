using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace ServiceContracts.DTO
{
    public class FileResponse
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Guid? VirualFolderId { get; set; }
        public string? VirualFolderName { get; set; }
        public string? AddedBy { get; set; }
        public FileResponse()
        {
            FileName = string.Empty;
            FilePath = string.Empty;
        }

        public FileDetails ToFileDetails()
        {
            return new FileDetails(FileName, FilePath, VirualFolderId.Value);
        }
    }

    public static class FileDetailsExtensions
    {
        public static FileResponse ToFileResponse(this FileDetails fileDetails)
        {
            return new FileResponse
            {
                Id = fileDetails.Id,
                FileName = fileDetails.FileName,
                FilePath = fileDetails.FilePath,
                VirualFolderId = fileDetails.VirualFolderId,
                AddedBy = fileDetails.AddedBy
            };
        }
    }
}
