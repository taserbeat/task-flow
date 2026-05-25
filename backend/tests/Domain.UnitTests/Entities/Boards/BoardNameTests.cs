using Domain.Entities.Boards;
using Domain.Exceptions;
using Xunit;
using Xunit.Abstractions;

namespace Domain.UnitTests.Entities.Boards
{
    /// <summary>
    /// <see cref="BoardName"/>のテスト
    /// </summary>
    public class BoardNameTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public BoardNameTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact(DisplayName = "ボード名の生成に成功する")]
        public void Constructor_Should_Success()
        {
            var name = new BoardName("テストボード");
            Assert.Equal("テストボード", name.Value);
        }

        [Theory(DisplayName = "ボード名の生成に失敗する (空文字または空白)")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("　")]
        [InlineData("　　")]
        public void Constructor_Should_Failed_When_EmptyOrWhitespace(string name)
        {
            _outputHelper.WriteLine($"入力値: {name}");
            Assert.Throws<AppValidateException>(() => new BoardName(name));
        }

        [Fact(DisplayName = "ボード名の生成に失敗する (最大文字数オーバー)")]
        public void Constructor_Should_Failed_When_Exceeds_MaxLength()
        {
            var longName = new string('a', BoardName.MaxLength + 1);
            Assert.Throws<AppValidateException>(() => new BoardName(longName));
        }
    }
}