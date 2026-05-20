using Domain.Entities.Common.ValueObjects;

namespace Domain.Entities.TaskItems
{
    /// <summary>
    /// タスクの位置を表す値オブジェクト
    /// </summary>
    /// <value></value>
    public record TaskItemPosition : IValueObject, IComparable<TaskItemPosition>
    {
        /// <summary>
        /// 既定の位置番号
        /// </summary>
        private const int DefaultPosition = 100;

        /// <summary>
        /// 既定の位置間隔
        /// </summary>
        private const int DefaultInterval = 100;

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

        /// <summary>
        /// 次の新しい位置を取得する
        /// </summary>
        /// <returns></returns>
        public TaskItemPosition NewNextPosition()
        {
            return new(Value + DefaultInterval);
        }

        /// <summary>
        /// 前の新しい位置を取得する
        /// </summary>
        /// <returns></returns>
        public TaskItemPosition NewPreviousPosition()
        {
            return new(Value / 2);
        }

        /// <summary>
        /// 中間の位置を取得する
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static TaskItemPosition NewMiddlePosition(TaskItemPosition low, TaskItemPosition high)
        {
            return new((low.Value + high.Value) / 2);
        }

        /// <summary>
        /// 新しい初期位置を生成する
        /// </summary>
        /// <returns></returns>
        public static TaskItemPosition NewInitPosition()
        {
            return new(DefaultPosition);
        }
    }
}