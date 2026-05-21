using Domain.Entities.Common.ValueObjects;
using Domain.Exceptions;

namespace Domain.Entities.Boards
{
    /// <summary>
    /// ボード名を表す値オブジェクト
    /// </summary>
    public record BoardName : IValueObject
    {
        public string Value { get; }

        public BoardName(string value)
        {
            if (!Validate(value))
            {
                throw new AppValidateException("ボード名を指定してください。");
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