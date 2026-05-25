using Domain.Entities.Common;
using Domain.Entities.Roles;
using Domain.Entities.TaskItems;
using Domain.Entities.Tenants;

namespace Domain.Entities.Users
{
    /// <summary>
    /// ユーザーのエンティティモデル
    /// </summary>
    public class UserEm : BaseTenantAuditableEm<UserId>
    {
        /// <summary>
        /// メールアドレス
        /// </summary>
        /// <value></value>
        public UserEmail Email { get; private set; } = default!;

        /// <summary>
        /// パスワードハッシュ
        /// </summary>
        /// <value></value>
        public UserPasswordHash PasswordHash { get; private set; } = default!;

        /// <summary>
        /// ユーザーの氏名
        /// </summary>
        /// <value></value>
        public UserName Username { get; private set; } = default!;

        /// <summary>
        /// ロールID
        /// </summary>
        /// <value></value>
        public RoleId RoleId { get; private set; }

        /// <summary>
        /// ユーザーが有効であるか?
        /// </summary>
        /// <value></value>
        public bool IsActive { get; private set; }

        /// <summary>
        /// テナントのナビゲーションプロパティ
        /// </summary>
        /// <value></value>
        public TenantEm Tenant { get; private set; } = default!;

        /// <summary>
        /// ロールのナビゲーションプロパティ
        /// </summary>
        /// <value></value>
        public RoleEm Role { get; private set; } = default!;

        /// <summary>
        /// 担当しているタスク一覧のナビゲーションプロパティ
        /// </summary>
        /// <typeparam name="TaskItemEm"></typeparam>
        /// <returns></returns>
        public ICollection<TaskItemEm> AssignedTasks { get; private set; } = new List<TaskItemEm>();

        public static UserEm Create(UserId userId, TenantId tenantId, DateTimeOffset createdAt, DateTimeOffset updatedAt, UserId? createdBy, UserId? updatedBy, UserEmail email, UserPasswordHash passwordHash, UserName username, RoleId roleId)
        {
            return new UserEm
            {
                Id = userId,
                TenantId = tenantId,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                CreatedBy = createdBy,
                UpdatedBy = updatedBy,
                Email = email,
                PasswordHash = passwordHash,
                Username = username,
                RoleId = roleId,
                IsActive = true,
            };
        }

        public void ChangeEmail(UserEmail newEmail, DateTimeOffset updatedAt, UserId? updatedBy)
        {
            Email = newEmail;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void ChangePassword(UserPasswordHash newPasswordHash, DateTimeOffset updatedAt, UserId? updatedBy)
        {
            PasswordHash = newPasswordHash;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void ChangeName(UserName newUsername, DateTimeOffset updatedAt, UserId? updatedBy)
        {
            Username = newUsername;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void Activate(DateTimeOffset updatedAt, UserId? updatedBy)
        {
            IsActive = true;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void Deactivate(DateTimeOffset updatedAt, UserId? updatedBy)
        {
            IsActive = false;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void ChangeRole(RoleId newRoleId, DateTimeOffset updatedAt, UserId? updatedBy)
        {
            RoleId = newRoleId;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        /// <summary>
        /// ロールのナビゲーションプロパティを設定する (テスト用)
        /// </summary>
        /// <param name="role"></param>
        public void SetRole(RoleEm role)
        {
            Role = role;
        }
    }
}