using Domain.Entities.Common.ValueObjects;

namespace Domain.Entities.BoardColumns
{
    /// <summary>
    /// ボード列の位置を表す値オブジェクト
    /// </summary>
    public record BoardColumnPosition : IValueObject
    {
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
    }
}