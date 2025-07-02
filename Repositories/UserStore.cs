using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class UserStore : IUserEmailStore<IdentityUser> , IUserPasswordStore<IdentityUser>
    {
        IUserRepository _userRepository;

        public UserStore(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (cancellationToken.IsCancellationRequested) return IdentityResult.Failed();
            return await _userRepository.CreateAsync(user, cancellationToken);
        }

        public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return await _userRepository.DeleteAsync(user, cancellationToken);
        }

        public void Dispose()
        {
            
        }

        public async Task<IdentityUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return await _userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);
        }

        public async Task<IdentityUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _userRepository.FindByIdAsync(userId,cancellationToken);
        }

        public async Task<IdentityUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await _userRepository.FindByNameAsync(normalizedUserName, cancellationToken);
        }

        public async Task<string?> GetEmailAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return await _userRepository.GetEmailAsync(user, cancellationToken);
        }

        public async Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return await _userRepository.GetEmailConfirmedAsync(user, cancellationToken);
        }

        public async Task<string?> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return await _userRepository.GetNormalizedEmailAsync(user, cancellationToken);
        }

        public async Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return await _userRepository.GetNormalizedUserNameAsync(user, cancellationToken);
        }

        public async Task<string?> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return await _userRepository.GetPasswordHashAsync(user, cancellationToken);
        }

        public async Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return user.Id ?? throw new ArgumentNullException(nameof(user.Id), "User ID cannot be null.");
        }

        public async Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return await _userRepository.GetUserNameAsync(user, cancellationToken);
        }

        public async Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return await _userRepository.HasPasswordAsync(user, cancellationToken);
        }

        public async Task SetEmailAsync(IdentityUser user, string? email, CancellationToken cancellationToken)
        {
            await _userRepository.SetEmailAsync(user, email, cancellationToken);
        }

        public async Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            await _userRepository.SetEmailConfirmedAsync(user, confirmed, cancellationToken);
        }

        public async Task SetNormalizedEmailAsync(IdentityUser user, string? normalizedEmail, CancellationToken cancellationToken)
        {
            await _userRepository.SetNormalizedEmailAsync(user, normalizedEmail, cancellationToken);
        }

        public async Task SetNormalizedUserNameAsync(IdentityUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            await _userRepository.SetNormalizedUserNameAsync(user, normalizedName, cancellationToken);
        }

        public async Task SetPasswordHashAsync(IdentityUser user, string? passwordHash, CancellationToken cancellationToken)
        {
            await _userRepository.SetPasswordHashAsync(user, passwordHash, cancellationToken);
        }

        public async Task SetUserNameAsync(IdentityUser user, string? userName, CancellationToken cancellationToken)
        {
            await _userRepository.SetUserNameAsync(user, userName, cancellationToken);
        }

        public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return await _userRepository.UpdateAsync(user, cancellationToken);
        }
    }
}
