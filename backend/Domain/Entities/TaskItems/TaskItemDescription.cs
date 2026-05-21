using Domain.Entities.Common.ValueObjects;
using Domain.Exceptions;

namespace Domain.Entities.TaskItems
{
    /// <summary>
    /// タスクの説明を表す値オブジェクト
    /// </summary>
    public class TaskItemDescription : IValueObject
    {
        public string Value { get; }

        public TaskItemDescription(string value)
        {
            if (!Validate(value))
            {
                throw new AppValidateException("説明が不正です。");
            }

            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        private static bool Validate(string value)
        {
            return true;
        }
    }
}