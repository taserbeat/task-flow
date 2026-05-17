using Domain.Entities.Common.ValueObjects;

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
            // タスクの説明として行こうか検証する処理を追加する

            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}