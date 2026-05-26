using Domain.Entities.TaskItems;
using Domain.Exceptions;
using Xunit;
using Xunit.Abstractions;

namespace Domain.UnitTests.Entities.TaskItems
{
    /// <summary>
    /// <see cref="TaskItemTitle"/>のテスト
    /// </summary>
    public class TaskItemTitleTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public TaskItemTitleTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact(DisplayName = "タイトルの生成に成功する")]
        public void Constructor_Should_Success()
        {
            var title = new TaskItemTitle("タスク1");
            Assert.Equal("タスク1", title.Value);
        }

        [Theory(DisplayName = "タイトルの生成に失敗する (空文字または空白)")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("　")]
        [InlineData("　　")]
        public void Constructor_Should_Failed_When_EmptyOrWhitespace(string title)
        {
            _outputHelper.WriteLine($"入力値: {title}");
            Assert.Throws<AppValidateException>(() => new TaskItemTitle(title));
        }

        [Fact(DisplayName = "タイトルの生成に失敗する (最大文字数オーバー)")]
        public void Constructor_Should_Failed_When_Exceeds_MaxLength()
        {
            var longTitle = new string('a', TaskItemTitle.MaxLength + 1);
            Assert.Throws<AppValidateException>(() => new TaskItemTitle(longTitle));
        }
    }
}
