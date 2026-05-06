using Domain.Entities.Common.ValueObjects;

namespace Domain.Entities.Users
{
    /// <summary>
    /// ユーザーのパスワードハッシュを表す値オブジェクト
    /// </summary>
    /// <value></value>
    public record UserPasswordHash : IValueObject
    {
        public string Value { get; }

        public UserPasswordHash(string value)
        {
            Value = value;
        }
    }
}