using Entities;
using System.Linq.Expressions;

namespace RepositoryContracts
{
    public interface IFileRepository
    {
        Task<FileDetails> AddFile(FileDetails fileDetails);
        Task<bool> DeleteFile(Guid fileId);
        Task<List<FileDetails>> ListFiles();
        Task<FileDetails?> GetFileDetails(Guid fileId);
        Task<FileDetails> UpdateFile(FileDetails fileDetails);
        Task<bool> Contains(Expression<Func<FileDetails,bool>> predicate);
    }
}
