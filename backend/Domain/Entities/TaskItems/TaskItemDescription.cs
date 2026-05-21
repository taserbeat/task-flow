using Domain.Entities.Common.ValueObjects;
using Domain.Exceptions;

namespace Domain.Entities.TaskItems
{
    /// <summary>
    /// タスクの説明を表す値オブジェクト
    /// </summary>
    public class TaskItemDescription : IValueObject
    {
        /// <summary>
        /// 最大文字数
        /// </summary>
        public const int MaxLength = 512;

        public string Value { get; }

        public TaskItemDescription(string value)
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
            if (value.Length > MaxLength)
            {
                throw new AppValidateException("説明の文字数がオーバーしています。");
            }
        }
    }
}