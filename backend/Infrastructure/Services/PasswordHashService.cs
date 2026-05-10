using Application.Services;
using Domain.Entities.Users;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    /// <summary>
    /// パスワードハッシュサービス
    /// </summary>
    public class PasswordHashService : IPasswordHashService
    {
        private readonly PasswordHasher<object> _hasher = new();

        public UserPasswordHash GenerateHash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new AppValidateException("パスワードが空です。");
            }

            var hash = _hasher.HashPassword(null!, password);
            return new UserPasswordHash(hash);
        }

        public bool VerifyPassword(string password, UserPasswordHash passwordHash)
        {
            var result = _hasher.VerifyHashedPassword(null!, passwordHash.Value, password);

            return result == PasswordVerificationResult.Success ||
                result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}