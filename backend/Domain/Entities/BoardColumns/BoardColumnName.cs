using Domain.Entities.Common.ValueObjects;

namespace Domain.Entities.BoardColumns
{
    /// <summary>
    /// ボード列名を表す値オブジェクト
    /// </summary>
    public record BoardColumnName : IValueObject
    {
        public string Value { get; }

        public BoardColumnName(string value)
        {
            // TODO: ボード名として有効か検証する処理を追加する

            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}