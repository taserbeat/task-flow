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
        public string Value { get; }

        public TaskItemTitle(string value)
        {
            if (!Validate(value))
            {
                throw new AppValidateException("タイトルを指定してください。");
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