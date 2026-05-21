using Domain.Entities.Common.ValueObjects;
using Domain.Exceptions;

namespace Domain.Entities.BoardColumns
{
    /// <summary>
    /// ボード列名を表す値オブジェクト
    /// </summary>
    public record BoardColumnName : IValueObject
    {
        public string Value { get; }

        public BoardColumnName(string value)
        {
            if (!Validate(value))
            {
                throw new AppValidateException("列名を指定してください。");
            }

            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        private static bool Validate(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}