using Domain.Entities.Common.ValueObjects;

namespace Domain.Entities.Users
{
    /// <summary>
    /// ユーザーのメールアドレスを表す値オブジェクト
    /// </summary>
    /// <value></value>
    public record UserEmail : IValueObject
    {
        public string Value { get; }

        public UserEmail(string value)
        {
            // TODO: メールアドレスとして有効な形式か検証処理を追加する

            Value = value;
        }
    }
}