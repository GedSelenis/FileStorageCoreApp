using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IVirtualFolderService
    {
        Task<FolderResponse> AddFolder(FolderAddRequest folderAddRequest);
        Task<List<FolderResponse>> GetAllFolders();
        Task<FolderResponse> GetFolderById(Guid id);
        Task<FolderResponse> UpdateFolder(FolderUpdateRequest folderUpdateRequest);
        Task<bool> DeleteFolder(Guid id);
        Task<FolderResponse> MoveToFolder(FolderToFolderRequest folderToFolderRequest);
    }
}
