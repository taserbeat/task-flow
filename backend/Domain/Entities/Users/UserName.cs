using Domain.Entities.Common.ValueObjects;
using Domain.Exceptions;

namespace Domain.Entities.Users
{
    /// <summary>
    /// 氏名の値オブジェクト
    /// </summary>
    public sealed class UserName : IValueObject
    {
        /// <summary>
        /// 姓の最大文字数
        /// </summary>
        public const int MaxLastNameLength = 32;

        /// <summary>
        /// 名の最大文字数
        /// </summary>
        public const int MaxFirstNameLength = 32;

        /// <summary>
        /// 姓
        /// </summary>
        /// <value></value>
        public string LastName { get; }

        /// <summary>
        /// 名
        /// </summary>
        /// <value></value>
        public string FirstName { get; }

        /// <summary>
        /// フルネーム
        /// </summary>
        /// <value></value>
        public string FullName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(LastName))
                {
                    return FirstName;
                }

                if (string.IsNullOrWhiteSpace(FirstName))
                {
                    return LastName;
                }

                return $"{LastName} {FirstName}";
            }
        }

        public UserName(string lastName, string firstName)
        {
            ValidateLastName(lastName);
            ValidateFirstName(firstName);

            LastName = lastName;
            FirstName = firstName;
        }

        private static void ValidateLastName(string value)
        {
            if (value.Length > MaxLastNameLength)
            {
                throw new AppValidateException("姓の文字数がオーバーしています。");
            }
        }

        private static void ValidateFirstName(string value)
        {
            if (value.Length > MaxFirstNameLength)
            {
                throw new AppValidateException("名の文字数がオーバーしています。");
            }
        }
    }
}