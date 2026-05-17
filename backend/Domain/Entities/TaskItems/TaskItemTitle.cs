using Domain.Entities.Common.ValueObjects;

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
            // タスクのタイトルとして行こうか検証する処理を追加する

            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}