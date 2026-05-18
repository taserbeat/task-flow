namespace Application.Services
{
    /// <summary>
    /// 例外を扱うサービス
    /// </summary>
    public interface IExceptionService
    {
        /// <summary>
        /// ユニーク制約に違反した例外であるかチェックする
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        bool IsUniqueConstraintViolation(Exception ex);

        /// <summary>
        /// 外部キー制約に違反した例外であるかチェックする
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        bool IsForeignKeyViolation(Exception ex);

        /// <summary>
        /// NOT NULL制約に違反した例外であるかチェックする
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        bool IsNotNullViolation(Exception ex);
    }
}