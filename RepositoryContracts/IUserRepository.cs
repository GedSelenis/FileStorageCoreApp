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
        Task SetNormalizedUserNameAsync(IdentityUser user, string? normalizedName, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken);
        Task SetEmailAsync(IdentityUser user, string? email, CancellationToken cancellationToken);
        Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken);
        Task SetNormalizedEmailAsync(IdentityUser user, string? normalizedEmail, CancellationToken cancellationToken);
        Task<string?> GetEmailAsync(IdentityUser user, CancellationToken cancellationToken);
        Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken cancellationToken);
        Task<string?> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken);
        Task<string?> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken);
        Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken);
        Task SetPasswordHashAsync(IdentityUser user, string? passwordHash, CancellationToken cancellationToken);
        Task<IdentityUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);
    }
}
