using Domain.Entities.Common.ValueObjects;

namespace Domain.Entities.BoardColumns
{
    /// <summary>
    /// ボード列の位置を表す値オブジェクト
    /// </summary>
    public record BoardColumnPosition : IValueObject, IComparable<BoardColumnPosition>
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

        public BoardColumnPosition(int value)
        {
            // TODO: ボード列の位置として有効か検証する処理を追加する

            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public int CompareTo(BoardColumnPosition? other)
        {
            return Value.CompareTo(other!.Value);
        }

        public static bool operator <(BoardColumnPosition left, BoardColumnPosition right)
        {
            return left.Value < right.Value;
        }

        public static bool operator >(BoardColumnPosition left, BoardColumnPosition right)
        {
            return left.Value > right.Value;
        }

        public static bool operator <=(BoardColumnPosition left, BoardColumnPosition right)
        {
            return left.Value <= right.Value;
        }

        public static bool operator >=(BoardColumnPosition left, BoardColumnPosition right)
        {
            return left.Value >= right.Value;
        }

        /// <summary>
        /// 次の新しい位置を取得する
        /// </summary>
        /// <returns></returns>
        public BoardColumnPosition NewNextPosition()
        {
            return new(Value + DefaultInterval);
        }

        /// <summary>
        /// 前の新しい位置を取得する
        /// </summary>
        /// <returns></returns>
        public BoardColumnPosition NewPreviousPosition()
        {
            return new(Value / 2);
        }

        /// <summary>
        /// 中間の位置を取得する
        /// </summary>
        /// <returns></returns>
        public static BoardColumnPosition NewMiddlePosition(BoardColumnPosition low, BoardColumnPosition high)
        {
            return new((low.Value + high.Value) / 2);
        }

        /// <summary>
        /// 新しい初期位置を取得する
        /// </summary>
        /// <returns></returns>
        public static BoardColumnPosition NewInitPosition()
        {
            return new(DefaultPosition);
        }
    }
}