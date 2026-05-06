namespace Domain.Helpers
{
    /// <summary>
    /// GUID操作のヘルパークラス
    /// </summary>
    public class GuidHelper
    {
        /// <summary>
        /// 新しいGUIDを生成する
        /// </summary>
        /// <returns></returns>
        public static Guid NewGuid()
        {
            return Guid.CreateVersion7();
        }
    }
}