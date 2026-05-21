using Domain.Entities.Common.ValueObjects;
using Domain.Exceptions;

namespace Domain.Entities.BoardColumns
{
    /// <summary>
    /// ボード列名を表す値オブジェクト
    /// </summary>
    public record BoardColumnName : IValueObject
    {
        /// <summary>
        /// 最大文字数
        /// </summary>
        public const int MaxLength = 128;

        public string Value { get; }

        public BoardColumnName(string value)
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
                throw new AppValidateException("列名を指定してください。");
            }

            if (value.Length > MaxLength)
            {
                throw new AppValidateException("列名の文字数がオーバーしています。");
            }
        }
    }
}