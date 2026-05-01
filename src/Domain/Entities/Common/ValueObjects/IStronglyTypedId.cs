namespace Domain.Entities.Common.ValueObjects
{
    /// <summary>
    /// 型指定されたIDを表すインターフェース
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStronglyTypedId<T> : IValueObject
    {
        T Value { get; }
    }
}