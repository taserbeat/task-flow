using System.Net.Mail;
using Domain.Entities.Common.ValueObjects;
using Domain.Exceptions;

namespace Domain.Entities.Users
{
    /// <summary>
    /// ユーザーのメールアドレスを表す値オブジェクト
    /// </summary>
    /// <value></value>
    public record UserEmail : IValueObject
    {
        /// <summary>
        /// 最大文字数
        /// </summary>
        public const int MaxLength = 256;

        public string Value { get; }

        public UserEmail(string value)
        {
            Validate(value);

            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        private static void Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new AppValidateException("メールアドレスを指定してください。"); ;
            }

            var normValue = value.Trim().ToLowerInvariant();

            if (normValue.Length > MaxLength)
            {
                throw new AppValidateException("メールアドレスの文字数がオーバーしています。");
            }

            // @が1個か?
            var atIndex = value.IndexOf('@');

            if (atIndex <= 0)
            {
                throw new AppValidateException("メールアドレスは'@'が1つのみである必要があります。");
            }

            if (atIndex != value.LastIndexOf('@'))
            {
                throw new AppValidateException("メールアドレスは'@'を末尾にできません。");
            }

            // ドメイン部
            var domain = value[(atIndex + 1)..];

            if (string.IsNullOrWhiteSpace(domain))
            {
                throw new AppValidateException("メールアドレスが不正な形式です。");
            }

            if (!domain.Contains('.'))
            {
                throw new AppValidateException("メールアドレスが不正な形式です。");
            }

            if (!MailAddress.TryCreate(value, out _))
            {
                throw new AppValidateException("メールアドレスが不正な形式です。");
            }
        }
    }
}