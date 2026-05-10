using Domain.Entities.Common.ValueObjects;
using Domain.Helpers;

namespace Domain.Entities.Users
{
    /// <summary>
    /// ユーザーIDの値オブジェクト
    /// </summary>
    public readonly record struct UserId(Guid Value) : IStronglyTypedId<Guid>
    {
        public static UserId New(Guid? guid = null)
        {
            return new UserId(guid ?? GuidHelper.NewGuid());
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}