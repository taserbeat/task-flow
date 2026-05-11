using System.Text.Json.Serialization;
using Domain.Entities.Roles;

namespace Web.Dtos.Roles
{
    /// <summary>
    /// ロールの詳細レスポンス
    /// </summary>
    public class RoleDetailResponse
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
        /// ラベル名
        /// </summary>
        /// <value></value>
        [JsonPropertyName("label")]
        public required string Label { get; set; }

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
        public static RoleDetailResponse FromEntity(RoleEm em)
        {
            return new RoleDetailResponse
            {
                Id = em.Id.Value,
                Name = em.Name,
                Label = em.Label.Value,
                Level = (int)em.Level,
            };
        }
    }
}