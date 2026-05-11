using System.Text.Json.Serialization;
using Domain.Entities.Roles;

namespace Web.Dtos.Roles
{
    /// <summary>
    /// ロール情報のレスポンス
    /// </summary>
    public class RoleResponse
    {
        /// <summary>
        /// ロールID
        /// </summary>
        /// <value></value>
        [JsonPropertyName("id")]
        public required Guid Id { get; set; }

        /// <summary>
        /// ロール名
        /// </summary>
        /// <value></value>
        [JsonPropertyName("name")]
        public required RoleNameEnum Name { get; set; }

        /// <summary>
        /// ロールレベル
        /// </summary>
        /// <value></value>
        [JsonPropertyName("level")]
        public required int Level { get; set; }

        /// <summary>
        /// エンティティからレスポンスを作成する
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        public static RoleResponse FromEntity(RoleEm em)
        {
            return new RoleResponse
            {
                Id = em.Id.Value,
                Name = em.Name,
                Level = (int)em.Level,
            };
        }
    }
}