using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Dtos.Users.UpdateUser
{
    /// <summary>
    /// ユーザーの更新リクエスト
    /// </summary>
    public record UpdateUserRequest
    {
        /// <summary>
        /// メールアドレス
        /// </summary>
        /// <value></value>
        public string? Email { get; set; }

        /// <summary>
        /// パスワード
        /// </summary>
        /// <value></value>
        public string? Password { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        /// <value></value>
        public string? LastName { get; init; }

        /// <summary>
        /// 名
        /// </summary>
        /// <value></value>
        public string? FirstName { get; init; }

        /// <summary>
        /// ロールID
        /// </summary>
        /// <value></value>
        public Guid? RoleId { get; init; }

        /// <summary>
        /// 有効フラグ
        /// </summary>
        /// <value></value>
        public bool? IsActive { get; init; }
    }
}