using Domain.Entities.Common.ValueObjects;
using Domain.Helpers;

namespace Domain.Entities.Auth
{
    /// <summary>
    /// セッションIDの値オブジェクト
    /// </summary>
    public readonly record struct SessionId(Guid Value) : IStronglyTypedId<Guid>
    {
        public static SessionId New(Guid? guid = null)
        {
            return new SessionId(guid ?? GuidHelper.NewGuid());
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}