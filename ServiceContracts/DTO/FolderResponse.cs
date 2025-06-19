using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class FolderResponse
    {
        public Guid Id { get; set; }
        public string? FolderName { get; set; }

        public FolderUpdateRequest ToFolderUpdateRequest()
        {
            return new FolderUpdateRequest()
            {
                Id = Id,
                FolderName = FolderName
            };
        }

    }

    public static class VirtualFolderExtensions
    {
        public static FolderResponse ToFolderResponse(this VirtualFolder virtualFolder)
        {
            return new FolderResponse
            {
                Id = virtualFolder.Id,
                FolderName = virtualFolder.FolderName
            };
        }
    }
}
