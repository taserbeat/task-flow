using Domain.Entities.Common.ValueObjects;
using Domain.Exceptions;

namespace Domain.Entities.TaskItems
{
    /// <summary>
    /// タスクのタイトルを表す値オブジェクト
    /// </summary>
    /// <value></value>
    public record TaskItemTitle : IValueObject
    {
        /// <summary>
        /// 最大文字数
        /// </summary>
        public const int MaxLength = 128;

        public string Value { get; }

        public TaskItemTitle(string value)
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
                throw new AppValidateException("タイトルを指定してください。");
            }

            if (value.Length > MaxLength)
            {
                throw new AppValidateException("タイトルの文字数がオーバーしています。");
            }
        }
    }
}