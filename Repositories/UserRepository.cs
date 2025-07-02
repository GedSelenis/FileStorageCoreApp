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
            User? existingUser = await _db.Users.FirstOrDefaultAsync(u => u.UserId == Guid.Parse(user.Id), cancellationToken);
            SecretUser? existingSecretUser = await _db.SecretUsers.FirstOrDefaultAsync(su => su.UserId == Guid.Parse(user.Id), cancellationToken);
            User user1 = user.ToUser();
            SecretUser secretUser = new SecretUser
            {
                Id = Guid.NewGuid(),
                PasswordHash = user.PasswordHash,
                UserId = user1.UserId
            };
            if (existingSecretUser != null && existingUser != null)
            {
                return IdentityResult.Success; // User already exists, no need to create again
            }
            if (existingUser != null)
            {  
                _db.SecretUsers.Add(secretUser);
                existingUser.SecretUserId = secretUser.Id;
            }
            else
            {
                _db.SecretUsers.Add(secretUser);
                _db.Users.Add(user1);
            }            
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

        public async Task<IdentityUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return await _db.Users
                .Where(u => u.NormalizedEmail == normalizedEmail)
                .Select(u => u.ToIdentityUser())
                .FirstOrDefaultAsync(cancellationToken);
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

        public async Task<string?> GetEmailAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return await _db.Users
                .Where(u => u.UserId == Guid.Parse(user.Id))
                .Select(u => u.Email)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return await _db.Users
                .Where(u => u.UserId == Guid.Parse(user.Id))
                .Select(u => u.EmailConfirmed)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<string?> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return await _db.Users
                .Where(u => u.UserId == Guid.Parse(user.Id))
                .Select(u => u.NormalizedEmail)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            User? userEntity = await _db.Users.FirstOrDefaultAsync(u => u.UserId == Guid.Parse(user.Id), cancellationToken);
            return userEntity?.NormalizedUserName;
        }

        public Task<string?> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return _db.SecretUsers
                .Where(su => su.UserId == Guid.Parse(user.Id))
                .Select(su => su.PasswordHash)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return await _db.Users
                .Where(u => u.UserId == Guid.Parse(user.Id))
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return await _db.SecretUsers
                .Where(su => su.UserId == Guid.Parse(user.Id))
                .Select(su => !string.IsNullOrEmpty(su.PasswordHash))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task SetEmailAsync(IdentityUser user, string? email, CancellationToken cancellationToken)
        {
            User? userEntity = _db.Users.FirstOrDefault(u => u.UserId == Guid.Parse(user.Id));
            if (userEntity == null)
            {
                throw new ArgumentNullException(nameof(user), "Could not find user.");
            }
            userEntity.Email = email;
            userEntity.NormalizedEmail = email?.ToUpperInvariant();
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            User? userEntity = _db.Users.FirstOrDefault(u => u.UserId == Guid.Parse(user.Id));
            if (userEntity == null)
            {
                throw new ArgumentNullException(nameof(user), "Could not find user.");
            }
            userEntity.EmailConfirmed = confirmed;
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task SetNormalizedEmailAsync(IdentityUser user, string? normalizedEmail, CancellationToken cancellationToken)
        {
            User? userEntity = _db.Users.FirstOrDefault(u => u.UserId == Guid.Parse(user.Id));
            if (userEntity == null)
            {
                throw new ArgumentNullException(nameof(user), "Could not find user.");
            }
            userEntity.NormalizedEmail = normalizedEmail;
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task SetNormalizedUserNameAsync(IdentityUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            User? userEntity = _db.Users.FirstOrDefault(u => u.UserId == Guid.Parse(user.Id));
            if (userEntity == null)
            {
                throw new ArgumentNullException(nameof(user), "Could not find user.");
            }
            userEntity.NormalizedUserName = normalizedName;
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task SetPasswordHashAsync(IdentityUser user, string? passwordHash, CancellationToken cancellationToken)
        {
            SecretUser? secretUser = _db.SecretUsers.FirstOrDefault(su => su.UserId == Guid.Parse(user.Id));
            User? userEntity = _db.Users.FirstOrDefault(u => u.UserId == Guid.Parse(user.Id));
            if (secretUser == null)
            {
                secretUser = new SecretUser
                {
                    Id = Guid.NewGuid(),
                    PasswordHash = passwordHash,
                    UserId = userEntity.UserId
                };
                userEntity.SecretUserId = secretUser.Id;
                _db.SecretUsers.Add(secretUser);
                await _db.SaveChangesAsync(cancellationToken);
                return;
            }
            secretUser.PasswordHash = passwordHash;
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
        {
            User? userEntity = _db.Users.FirstOrDefault(u => u.UserId == Guid.Parse(user.Id));
            if (userEntity == null)
            {
                //throw new ArgumentNullException(nameof(user), "Could not find user.");
                userEntity = user.ToUser();
                userEntity.UserName = userName;
                _db.Users.Add(userEntity);
                await _db.SaveChangesAsync(cancellationToken);
                return;
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
