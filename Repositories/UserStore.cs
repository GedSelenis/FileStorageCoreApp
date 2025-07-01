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
    public class UserStore : IUserStore<IdentityUser>
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
            throw new NotImplementedException();
        }

        public async Task<IdentityUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _userRepository.FindByIdAsync(userId,cancellationToken);
        }

        public async Task<IdentityUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await _userRepository.FindByNameAsync(normalizedUserName, cancellationToken);
        }

        public async Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return await _userRepository.GetNormalizedUserNameAsync(user, cancellationToken);
        }

        public async Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return await _userRepository.GetUserNameAsync(user, cancellationToken);
        }

        public Task SetNormalizedUserNameAsync(IdentityUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
