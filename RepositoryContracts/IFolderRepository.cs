using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts
{
    public interface IFolderRepository
    {
        Task<VirtualFolder> AddFolder(VirtualFolder folderDetails);
        Task<bool> DeleteFolder(Guid folderId);
        Task<List<VirtualFolder>> ListFolders();
        Task<VirtualFolder?> GetFolder(Guid folderId);
        Task<VirtualFolder> UpdateFolder(VirtualFolder folderDetails);
    }
}
