using Domain.Entities.Common.ValueObjects;
using Domain.Helpers;

namespace Domain.Entities.BoardColumns
{
    /// <summary>
    /// ボード列IDの値オブジェクト
    /// </summary>
    public readonly record struct BoardColumnId(Guid Value) : IStronglyTypedId<Guid>
    {
        public static BoardColumnId New(Guid? guid = null)
        {
            return new BoardColumnId(guid ?? GuidHelper.NewGuid());
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}