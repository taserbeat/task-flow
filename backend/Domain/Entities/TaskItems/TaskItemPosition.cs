using Domain.Entities.Common.ValueObjects;

namespace Domain.Entities.TaskItems
{
    /// <summary>
    /// タスクの位置を表す値オブジェクト
    /// </summary>
    /// <value></value>
    public record TaskItemPosition : IValueObject
    {
        public int Value { get; }

        public TaskItemPosition(int value)
        {
            // TODO: タスクの位置として有効か検証する処理を追加する

            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}