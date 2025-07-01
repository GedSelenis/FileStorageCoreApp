using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts
{
    public interface IUserRepository
    {
        Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken);
        Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken);
        Task<IdentityUser?> FindByIdAsync(string userId, CancellationToken cancellationToken);
        Task<IdentityUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken);
        Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken);
        Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken);
        Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken);
    }
}
