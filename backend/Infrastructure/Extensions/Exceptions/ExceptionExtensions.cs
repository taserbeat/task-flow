using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Extensions.Exceptions
{
    /// <summary>
    /// <see cref="Exception"/> の拡張メソッド
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// ユニーク制約に違反した例外であるかチェックする
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsUniqueConstraintViolation(this Exception ex)
        {
            var dbEx = GetDbException(ex);

            if (dbEx is PostgresException pgEx)
            {
                return pgEx.SqlState == "23505";
            }

            return false;
        }

        /// <summary>
        /// 外部キー制約に違反した例外であるかチェックする
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsForeignKeyViolation(this Exception ex)
        {
            var dbEx = GetDbException(ex);

            if (dbEx is PostgresException pgEx)
            {
                return pgEx.SqlState == "23503";
            }

            return false;
        }

        /// <summary>
        /// NOT NULL制約に違反した例外であるかチェックする
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsNotNullViolation(this Exception ex)
        {
            var dbEx = GetDbException(ex);

            if (dbEx is PostgresException pgEx)
            {
                return pgEx.SqlState == "23502";
            }

            return false;
        }

        /// <summary>
        /// 違反した制約の名称を取得する
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>違反した制約名 (違反が無い場合はnull)</returns>
        public static string? GetConstraintName(this Exception ex)
        {
            var dbEx = GetDbException(ex);

            if (dbEx is PostgresException pgEx)
            {
                return pgEx.ConstraintName;
            }

            return null;
        }

        /// <summary>
        /// <see cref="DbException"/> を取得する
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static DbException? GetDbException(Exception ex)
        {
            if (ex is DbUpdateException dbUpdateEx)
            {
                return dbUpdateEx.InnerException as DbException;
            }

            if (ex is DbException dbEx)
            {
                return dbEx;
            }

            return null;
        }
    }
}