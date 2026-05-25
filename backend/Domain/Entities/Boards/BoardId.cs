using Domain.Entities.Common.ValueObjects;
using Domain.Helpers;

namespace Domain.Entities.Boards
{
    /// <summary>
    /// ボードIDの値オブジェクト
    /// </summary>
    public readonly record struct BoardId(Guid Value) : IStronglyTypedId<Guid>
    {
        public static BoardId New(Guid? guid = null)
        {
            return new BoardId(guid ?? GuidHelper.NewGuid());
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}