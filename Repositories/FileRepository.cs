using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories
{
    public class FileRepository : IFileRepository
    {
        ApplicationDbContext _db;

        public FileRepository(ApplicationDbContext applicationDbContext)
        {
            _db = applicationDbContext;
        }
        public async Task<FileDetails> AddFile(FileDetails fileDetails)
        {
            _db.FileDetails.Add(fileDetails);
            await _db.SaveChangesAsync();
            return fileDetails;
        }

        public async Task<bool> DeleteFile(Guid fileId)
        {
            _db.FileDetails.RemoveRange(_db.FileDetails.Where(f => f.Id == fileId));
            int changesMade = await _db.SaveChangesAsync();
            return changesMade > 0;
        }

        public async Task<FileDetails?> GetFileDetails(Guid fileId)
        {
            return await _db.FileDetails.FirstOrDefaultAsync(f => f.Id == fileId);
        }

        public async Task<List<FileDetails>> ListFiles()
        {
            return await _db.FileDetails.ToListAsync();
        }

        public async Task<FileDetails> UpdateFile(FileDetails fileDetails)
        {
            FileDetails? existingFile = await _db.FileDetails.FirstOrDefaultAsync(f => f.Id == fileDetails.Id);
            if (existingFile == null)
            {
                throw new ArgumentException("File not found.");
            }
            existingFile.FileName = fileDetails.FileName;
            existingFile.FilePath = fileDetails.FilePath;
            existingFile.VirualFolderId = fileDetails.VirualFolderId;
            await _db.SaveChangesAsync();
            return existingFile;
        }
    }
}
