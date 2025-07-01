using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            User user1 = user.ToUser();
            SecretUser secretUser = new SecretUser
            {
                Id = Guid.NewGuid(),
                PasswordHash = user.PasswordHash,
                UserId = user1.UserId
            };
            _db.SecretUsers.Add(secretUser);
            _db.Users.Add(user1);
            var result = await _db.SaveChangesAsync(cancellationToken);
            if (result > 0)
            {
                return IdentityResult.Success;
            }
            else
                return IdentityResult.Failed(new IdentityError { Description = "Failed to create user." });
        }

        public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(user.Id);
            _db.Users.RemoveRange(_db.Users.Where(u => u.UserId == userId));
            _db.SecretUsers.RemoveRange(_db.SecretUsers.Where(su => su.UserId == userId));
            var result = await _db.SaveChangesAsync(cancellationToken);
            if (result > 0)
            {
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed(new IdentityError { Description = "Failed to delete user." });
            }
        }

        public async Task<IdentityUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            Guid id = Guid.Parse(userId);
            return (await _db.Users.Include("SecretUser").FirstOrDefaultAsync(u => u.UserId == id, cancellationToken))?.ToIdentityUser();
        }

        public async Task<IdentityUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await _db.Users
                .Where(u => u.NormalizedUserName == normalizedUserName)
                .Select(u => u.ToIdentityUser())
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            User? userEntity = await _db.Users.FirstOrDefaultAsync(u => u.UserId == Guid.Parse(user.Id), cancellationToken);
            return userEntity?.NormalizedUserName;
        }

        public async Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return await _db.Users
                .Where(u => u.UserId == Guid.Parse(user.Id))
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();
        }

        public async Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
        {
            User? userEntity = _db.Users.FirstOrDefault(u => u.UserId == Guid.Parse(user.Id));
            if (userEntity == null)
            {
                throw new ArgumentNullException(nameof(user), "Could not find user.");
            }
            userEntity.UserName = userName;
            userEntity.NormalizedUserName = userName?.ToUpperInvariant();
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(user.Id);
            User? userDb = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            SecretUser? secretUserDb = await _db.SecretUsers.FirstOrDefaultAsync(su => su.UserId == userId, cancellationToken);
            if (userDb == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            if (secretUserDb == null) return IdentityResult.Failed(new IdentityError { Description = "Secret user not found." });
            userDb.UserName = user.UserName;
            userDb.Email = user.Email;
            userDb.NormalizedUserName = user.NormalizedUserName;
            userDb.NormalizedEmail = user.NormalizedEmail;
            secretUserDb.PasswordHash = user.PasswordHash;

            await _db.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }
    }
}
