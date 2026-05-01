using Domain.Entities.Users;

namespace Application.Repositories
{
    /// <summary>
    /// ユーザーのリポジトリ
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// ユーザーを追加する
        /// </summary>
        /// <param name="userEm"></param>
        /// <returns></returns>
        Task AddAsync(UserEm userEm);

        /// <summary>
        /// 指定のユーザーを取得する
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns></returns>
        Task<UserEm?> GetByIdAsync(UserId userId);

        /// <summary>
        /// 指定のメールアドレスから有効なユーザーを取得する
        /// </summary>
        /// <param name="email">メールアドレス</param>
        /// <returns></returns>
        Task<UserEm?> GetActiveByEmailAsync(UserEmail email);
    }
}