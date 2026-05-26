using Domain.Entities.TaskItems;
using Domain.Exceptions;
using Xunit;

namespace Domain.UnitTests.Entities.TaskItems
{
    /// <summary>
    /// <see cref="TaskItemDescription"/>のテスト
    /// </summary>
    public class TaskItemDescriptionTests
    {
        [Fact(DisplayName = "説明の生成に成功する")]
        public void Constructor_Should_Success()
        {
            var description = new TaskItemDescription("タスクの説明");
            Assert.Equal("タスクの説明", description.Value);
        }

        [Fact(DisplayName = "説明の生成に成功する (空文字)")]
        public void Constructor_Should_Success_When_Empty()
        {
            var description = new TaskItemDescription("");
            Assert.Equal("", description.Value);
        }

        [Fact(DisplayName = "説明の生成に失敗する (最大文字数オーバー)")]
        public void Constructor_Should_Failed_When_Exceeds_MaxLength()
        {
            var longDescription = new string('a', TaskItemDescription.MaxLength + 1);
            Assert.Throws<AppValidateException>(() => new TaskItemDescription(longDescription));
        }
    }
}
