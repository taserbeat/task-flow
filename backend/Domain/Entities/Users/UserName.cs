using Domain.Entities.Common.ValueObjects;

namespace Domain.Entities.Users
{
    /// <summary>
    /// 氏名の値オブジェクト
    /// </summary>
    public sealed class UserName : IValueObject
    {
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
            LastName = lastName;
            FirstName = firstName;
        }
    }
}