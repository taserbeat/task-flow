using Domain.Entities.Users;
using Application.Exceptions;

namespace Application.Services
{
    /// <summary>
    /// パスワードハッシュサービス
    /// </summary>
    public interface IPasswordHashService
    {
        /// <summary>
        /// パスワードのハッシュを生成する
        /// </summary>
        /// <param name="password">パスワード</param>
        /// <returns></returns>
        /// <exception cref="AppValidateException" />
        UserPasswordHash GenerateHash(string password);

        /// <summary>
        /// パスワードを検証する
        /// </summary>
        /// <param name="password">パスワード</param>
        /// <param name="passwordHash">ハッシュ化されたパスワード</param>
        /// <returns></returns>
        bool VerifyPassword(string password, UserPasswordHash passwordHash);
    }
}