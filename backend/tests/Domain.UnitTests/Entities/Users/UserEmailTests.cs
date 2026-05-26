using Domain.Entities.Users;
using Domain.Exceptions;
using Xunit;
using Xunit.Abstractions;

namespace Domain.UnitTests.Entities.Users
{
    /// <summary>
    /// <see cref="UserEmail"/>のテスト
    /// </summary>
    public class UserEmailTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public UserEmailTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact(DisplayName = "E メールアドレスの生成に成功する")]
        public void Constructor_Should_Success()
        {
            var email = new UserEmail("test@example.com");
            Assert.Equal("test@example.com", email.Value);
        }

        [Theory(DisplayName = "E メールアドレスの生成に失敗する (空文字または空白)")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("　")]
        [InlineData("　　")]
        public void Constructor_Should_Failed_When_EmptyOrWhitespace(string email)
        {
            _outputHelper.WriteLine($"入力値: {email}");
            Assert.Throws<AppValidateException>(() => new UserEmail(email));
        }

        [Fact(DisplayName = "E メールアドレスの生成に失敗する (最大文字数オーバー)")]
        public void Constructor_Should_Failed_When_Exceeds_MaxLength()
        {
            var longEmail = new string('a', UserEmail.MaxLength + 1 - "@example.com".Length) + "@example.com";
            Assert.Throws<AppValidateException>(() => new UserEmail(longEmail));
        }

        [Theory(DisplayName = "E メールアドレスの生成に失敗する (不正な形式)")]
        [InlineData("testexample.com")]
        [InlineData("test@")]
        [InlineData("@example.com")]
        [InlineData("test@@example.com")]
        public void Constructor_Should_Failed_When_InvalidFormat(string email)
        {
            _outputHelper.WriteLine($"入力値: {email}");
            Assert.Throws<AppValidateException>(() => new UserEmail(email));
        }
    }
}