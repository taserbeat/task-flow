using Domain.Entities.Common.ValueObjects;
using Domain.Helpers;

namespace Domain.Entities.Roles
{
    public readonly record struct RoleId(Guid Value) : IStronglyTypedId<Guid>
    {
        public static RoleId New(Guid? guid = null)
        {
            return new RoleId(guid ?? GuidHelper.NewGuid());
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}