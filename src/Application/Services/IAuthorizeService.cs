using Domain.Entities.Roles;

namespace Application.Services
{
    /// <summary>
    /// 認可サービス
    /// </summary>
    public interface IAuthorizeService
    {
        /// <summary>
        /// 必須権限を指定し、ユーザーがその権限を所有しているか判定する
        /// </summary>
        /// <param name="requiredLevel">必須権限</param>
        /// <returns></returns>
        bool HasRequiredRole(RoleLevelEnum requiredLevel);
    }
}