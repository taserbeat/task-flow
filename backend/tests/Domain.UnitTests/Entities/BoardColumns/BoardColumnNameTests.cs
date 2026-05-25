using Domain.Entities.BoardColumns;
using Domain.Exceptions;
using Xunit;
using Xunit.Abstractions;

namespace Domain.UnitTests.Entities.BoardColumns
{
    /// <summary>
    /// <see cref="BoardColumnName"/>のテスト
    /// </summary>
    public class BoardColumnNameTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public BoardColumnNameTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact(DisplayName = "列名の生成に成功する")]
        public void Constructor_Should_Success()
        {
            var name = new BoardColumnName("ToDo");
            Assert.Equal("ToDo", name.Value);
        }

        [Theory(DisplayName = "列名の生成に失敗する (空文字または空白)")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("　")]
        [InlineData("　　")]
        public void Constructor_Should_Failed_When_EmptyOrWhitespace(string name)
        {
            _outputHelper.WriteLine($"入力値: {name}");
            Assert.Throws<AppValidateException>(() => new BoardColumnName(name));
        }

        [Fact(DisplayName = "列名の生成に失敗する (最大文字数オーバー)")]
        public void Constructor_Should_Failed_When_Exceeds_MaxLength()
        {
            var longName = new string('a', BoardColumnName.MaxLength + 1);
            Assert.Throws<AppValidateException>(() => new BoardColumnName(longName));
        }
    }
}
