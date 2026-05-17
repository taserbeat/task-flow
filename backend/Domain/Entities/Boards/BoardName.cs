using Domain.Entities.Common.ValueObjects;

namespace Domain.Entities.Boards
{
    /// <summary>
    /// ボード名を表す値オブジェクト
    /// </summary>
    public record BoardName : IValueObject
    {
        public string Value { get; }

        public BoardName(string value)
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