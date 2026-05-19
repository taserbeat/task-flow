using Domain.Entities.Common.ValueObjects;

namespace Domain.Entities.TaskItems
{
    /// <summary>
    /// タスクの位置を表す値オブジェクト
    /// </summary>
    /// <value></value>
    public record TaskItemPosition : IValueObject, IComparable<TaskItemPosition>
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

        public int CompareTo(TaskItemPosition? other)
        {
            return Value.CompareTo(other!.Value);
        }

        public static bool operator <(TaskItemPosition left, TaskItemPosition right)
        {
            return left.Value < right.Value;
        }

        public static bool operator >(TaskItemPosition left, TaskItemPosition right)
        {
            return left.Value > right.Value;
        }

        public static bool operator <=(TaskItemPosition left, TaskItemPosition right)
        {
            return left.Value <= right.Value;
        }

        public static bool operator >=(TaskItemPosition left, TaskItemPosition right)
        {
            return left.Value >= right.Value;
        }
    }
}