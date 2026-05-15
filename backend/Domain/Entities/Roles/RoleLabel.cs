using Domain.Entities.Common.ValueObjects;

namespace Domain.Entities.Roles
{
    /// <summary>
    /// ロールの表示名を表す値オブジェクト
    /// </summary>
    public class RoleLabel : IValueObject
    {
        public string Value { get; }

        public RoleLabel(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}