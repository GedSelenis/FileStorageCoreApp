using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class FolderRepository : IFolderRepository
    {
        ApplicationDbContext _db;

        public FolderRepository(ApplicationDbContext applicationDbContext)
        {
            _db = applicationDbContext;
        }

        public async Task<VirtualFolder> AddFolder(VirtualFolder folderDetails)
        {
            _db.VirtualFolders.Add(folderDetails);
            await _db.SaveChangesAsync();
            return folderDetails;
        }

        public async Task<bool> ContainsName(string folderName)
        {
            return await _db.VirtualFolders.AnyAsync(f => f.FolderName == folderName);
        }

        public async Task<bool> DeleteFolder(Guid folderId)
        {
            _db.VirtualFolders.RemoveRange(_db.VirtualFolders.Where(f => f.Id == folderId));
            int changesMade = await _db.SaveChangesAsync();
            return changesMade > 0;
        }

        public async Task<VirtualFolder?> GetFolder(Guid folderId)
        {
            return await _db.VirtualFolders.FirstOrDefaultAsync(f => f.Id == folderId);
        }

        public async Task<List<VirtualFolder>> ListFolders()
        {
            return await _db.VirtualFolders.ToListAsync();
        }

        public async Task<VirtualFolder> UpdateFolder(VirtualFolder folderDetails)
        {
            VirtualFolder? existingFolder = _db.VirtualFolders.FirstOrDefault(f => f.Id == folderDetails.Id);
            if (existingFolder == null)
            {
                throw new ArgumentException("Folder not found.");
            }
            existingFolder.FolderName = folderDetails.FolderName;
            existingFolder.ParentFolderId = folderDetails.ParentFolderId;
            await _db.SaveChangesAsync();
            return existingFolder;
        }
    }
}
