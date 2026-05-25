using Domain.Entities.Common.ValueObjects;
using Domain.Helpers;

namespace Domain.Entities.TaskItems
{
    /// <summary>
    /// タスクIDの値オブジェクト
    /// </summary>
    public readonly record struct TaskItemId(Guid Value) : IStronglyTypedId<Guid>
    {
        public static TaskItemId New(Guid? guid = null)
        {
            return new TaskItemId(guid ?? GuidHelper.NewGuid());
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}