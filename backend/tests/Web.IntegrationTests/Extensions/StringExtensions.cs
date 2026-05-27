using System.Text;
using System.Web;

namespace Web.IntegrationTests.Extensions
{
    /// <summary>
    /// URL文字列の制御を行うための、string型の拡張メソッド
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// URL 文字列にクエリパラメータを追加する
        /// </summary>
        /// <param name="self"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string AddQueryParameters(this string self, Dictionary<string, string> parameters)
        {
            var query = HttpUtility.ParseQueryString("");

            foreach (var (key, value) in parameters)
            {
                query.Add(key, value);
            }

            if (self.Contains('?'))
            {
                return self + "&" + query.ToString();
            }

            return self + "?" + query.ToString();
        }

        /// <summary>
        /// URL 文字列にクエリパラメータを追加する
        /// </summary>
        /// <param name="self"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string AddQueryParameters<T>(this string self, T parameters)
        {
            var query = HttpUtility.ParseQueryString("");

            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                var key = propertyInfo.Name;
                var value = propertyInfo.GetValue(parameters);

                query.Add(key, value?.ToString() ?? "");
            }

            if (self.Contains('?'))
            {
                return self + "&" + query.ToString();
            }

            return self + "?" + query.ToString();
        }

        /// <summary>
        /// パスパラメータの置換
        /// </summary>
        /// <param name="self"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string ReplacePathParameters<T>(this string self, T parameters)
        {
            var replacedPath = self;

            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                var key = propertyInfo.Name;
                var value = propertyInfo.GetValue(parameters);

                replacedPath = replacedPath.Replace("{{" + key + "}}", value?.ToString() ?? "");
            }

            return replacedPath;
        }

        /// <summary>
        /// 自身の文字列を <see cref="count"/> 回繰り返した文字列を取得する
        /// </summary>
        /// <param name="self"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string RepeatJoin(this string self, int count)
        {
            return Enumerable.Repeat(self, count)
                .Aggregate(new StringBuilder(), (s, c) => s.Append(c))
                .ToString();
        }
    }
}