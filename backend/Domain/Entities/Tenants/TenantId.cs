using Domain.Entities.Common.ValueObjects;
using Domain.Helpers;

namespace Domain.Entities.Tenants
{
    public readonly record struct TenantId(Guid Value) : IStronglyTypedId<Guid>
    {
        public static TenantId New(Guid? guid = null)
        {
            return new TenantId(guid ?? GuidHelper.NewGuid());
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}